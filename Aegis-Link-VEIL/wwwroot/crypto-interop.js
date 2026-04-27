window.aegisCrypto = {

    generateKeypair: function () {
        const kp = nacl.box.keyPair();

        const toBase64 = (bytes) => btoa(String.fromCharCode(...bytes));

        return {
            publicKey: toBase64(kp.publicKey),
            secretKey: toBase64(kp.secretKey)
        };
    },

    createAegisId: async function (publicKeyB64) {
        const ALPHABET = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ234567';

        const binaryStr = atob(publicKeyB64);
        const bytes = new Uint8Array(binaryStr.length);
        for (let i = 0; i < binaryStr.length; i++) {
            bytes[i] = binaryStr.charCodeAt(i);
        }

        const hashBuffer = await crypto.subtle.digest('SHA-256', bytes);
        const hash = new Uint8Array(hashBuffer);

        const slice = hash.slice(0, 5);
        const bits = Array.from(slice).map(b => b.toString(2).padStart(8, '0')).join('');

        let id = '';
        for (let i = 0; i < 40; i += 5) {
            id += ALPHABET[parseInt(bits.slice(i, i + 5), 2)];
        }
        return id;
    },

    saveIdentity: function (aegisId, publicKeyB64, secretKeyB64) {
        localStorage.setItem('aegis_identity', JSON.stringify({
            aegisId,
            publicKey: publicKeyB64,
            secretKey: secretKeyB64
        }));
    },

    loadIdentity: function () {
        const raw = localStorage.getItem('aegis_identity');
        return raw ? JSON.parse(raw) : null;
    },

    clearIdentity: function () {
        localStorage.removeItem('aegis_identity');
    },

    // Remove only Aegis Link data from this device.
    wipeDevice: function () {
        const aegisStorageKeys = ['aegis_identity', 'aegis_chats'];

        aegisStorageKeys.forEach((key) => {
            localStorage.removeItem(key);
            sessionStorage.removeItem(key);
        });
    },

    deriveKey: async function (sessionKeyString) {
        const encoder = new TextEncoder();
        const keyData = encoder.encode(sessionKeyString);
        const hashBuffer = await crypto.subtle.digest('SHA-256', keyData);
        return new Uint8Array(hashBuffer);
    },

    encryptMessage: async function (plaintext, sessionKeyString) {
        const keyBytes = await window.aegisCrypto.deriveKey(sessionKeyString);
        const nonce = nacl.randomBytes(nacl.secretbox.nonceLength);
        const messageBytes = nacl.util.decodeUTF8(plaintext);
        const encrypted = nacl.secretbox(messageBytes, nonce, keyBytes);
        const combined = new Uint8Array(nonce.length + encrypted.length);
        combined.set(nonce);
        combined.set(encrypted, nonce.length);
        return btoa(String.fromCharCode(...combined));
    },

    decryptMessage: async function (ciphertextB64, sessionKeyString) {
        try {
            const keyBytes = await window.aegisCrypto.deriveKey(sessionKeyString);
            const combined = Uint8Array.from(atob(ciphertextB64), c => c.charCodeAt(0));
            const nonce = combined.slice(0, nacl.secretbox.nonceLength);
            const ciphertext = combined.slice(nacl.secretbox.nonceLength);
            const decrypted = nacl.secretbox.open(ciphertext, nonce, keyBytes);
            if (!decrypted) return null;
            return nacl.util.encodeUTF8(decrypted);
        } catch {
            return null;
        }
    },

    generateSessionKey: function () {
        const bytes = new Uint8Array(16);
        crypto.getRandomValues(bytes);
        return Array.from(bytes, b => b.toString(16).padStart(2, '0')).join('');
    },

    // Derive an ECDH shared secret from own secret key + partner public key.
    // Both parties produce the same 32-byte key without ever transmitting it.
    computeSharedKey: function (mySecretKeyB64, partnerPublicKeyB64) {
        const mySecretKey = Uint8Array.from(atob(mySecretKeyB64), c => c.charCodeAt(0));
        const partnerPublicKey = Uint8Array.from(atob(partnerPublicKeyB64), c => c.charCodeAt(0));
        const sharedKey = nacl.box.before(partnerPublicKey, mySecretKey);
        return btoa(String.fromCharCode(...sharedKey));
    },

    // Encrypt plaintext with the ECDH-derived shared key (XSalsa20-Poly1305).
    // Returns base64(nonce || ciphertext).
    boxEncrypt: function (plaintext, sharedKeyB64) {
        const keyBytes = Uint8Array.from(atob(sharedKeyB64), c => c.charCodeAt(0));
        const nonce = nacl.randomBytes(nacl.secretbox.nonceLength);
        const messageBytes = nacl.util.decodeUTF8(plaintext);
        const encrypted = nacl.secretbox(messageBytes, nonce, keyBytes);
        const combined = new Uint8Array(nonce.length + encrypted.length);
        combined.set(nonce);
        combined.set(encrypted, nonce.length);
        return btoa(String.fromCharCode(...combined));
    },

    // Derive a deterministic room ID from two Aegis IDs.
    // IDs are normalised (trimmed and uppercased) then sorted before hashing so both parties
    // always compute the same room ID regardless of who initiates or how the ID was typed.
    // Returns a 32-char hex string (first 16 bytes of SHA-256).
    computeRoomId: async function (myAegisId, partnerAegisId) {
        const sorted = [myAegisId.trim().toUpperCase(), partnerAegisId.trim().toUpperCase()].sort().join(':');
        const encoder = new TextEncoder();
        const hashBuffer = await crypto.subtle.digest('SHA-256', encoder.encode(sorted));
        const hash = new Uint8Array(hashBuffer);
        return Array.from(hash.slice(0, 16), b => b.toString(16).padStart(2, '0')).join('');
    },

    // Decrypt a boxEncrypt payload. Returns plaintext or null on failure.
    boxDecrypt: function (ciphertextB64, sharedKeyB64) {
        try {
            const keyBytes = Uint8Array.from(atob(sharedKeyB64), c => c.charCodeAt(0));
            const combined = Uint8Array.from(atob(ciphertextB64), c => c.charCodeAt(0));
            const nonce = combined.slice(0, nacl.secretbox.nonceLength);
            const ciphertext = combined.slice(nacl.secretbox.nonceLength);
            const decrypted = nacl.secretbox.open(ciphertext, nonce, keyBytes);
            if (!decrypted) return null;
            return nacl.util.encodeUTF8(decrypted);
        } catch {
            return null;
        }
    },

    // loads settings from local storage
    loadSettings: function () {
        const raw = localStorage.getItem('aegis_settings');
        return raw ? JSON.parse(raw) : null;
    },

    // saves settings or updates
    saveSettings: function (settings) {
        localStorage.setItem('aegis_settings', JSON.stringify(settings));
    },

    // removes settings
    deleteSettings: function () {
        localStorage.removeItem('aegis_settings');
    },

    // loads contacts from local storage
    loadContacts: function () {
        const raw = localStorage.getItem('aegis_contacts');
        return raw ? JSON.parse(raw) : [];
    },

    // saves a new contract or updates an existing one
    saveContact: function (contact) {
        const contacts = this.loadContacts();
        const existingIndex = contacts.findIndex(c => c.aegisId === contact.aegisId);
        if (existingIndex >= 0) {
            contacts[existingIndex] = contact;
        } else {
            contacts.push(contact);
        }
        localStorage.setItem('aegis_contacts', JSON.stringify(contacts));
    },

    // removes a contact by their Aegis ID
    deleteContact: function (aegisId) {
        const contacts = this.loadContacts();
        const filtered = contacts.filter(c => c.aegisId !== aegisId);
        localStorage.setItem('aegis_contacts', JSON.stringify(filtered));
    },

    // generates a consistent colour for an id avatar
    colourForId: function (aegisId) {
        const colours = [
            '#6c63ff', '#4ecdc4', '#f7b731', '#fc5c65',
            '#45aaf2', '#a55eea', '#2bcbba', '#fd9644'
        ];
        let hash = 0;
        for (let i = 0; i < aegisId.length; i++) {
            hash = aegisId.charCodeAt(i) + ((hash << 5) - hash);
        }
        return colours[Math.abs(hash) % colours.length];
    }
};