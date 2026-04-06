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
    }
};