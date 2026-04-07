using AegisLink.Shared;
using Microsoft.JSInterop;

namespace Aegis_Link_VEIL.Services;

public class ContactService
{
    private readonly IJSRuntime _js;

    public ContactService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<List<Contact>> GetAllAsync()
    {
        var contacts = await _js.InvokeAsync<List<Contact>>("aegisCrypto.loadContacts");
        return contacts.OrderBy(c => c.Nickname).ToList();
    }

    public async Task<Contact?> GetByIdAsync(string aegisId)
    {
        var contacts = await _js.InvokeAsync<List<Contact>>("aegisCrypto.loadContacts");
        return contacts.FirstOrDefault(c => c.AegisId == aegisId);
    }

    public async Task SaveAsync(Contact contact)
    {
        await _js.InvokeVoidAsync("aegisCrypto.saveContact", contact);
    }

    public async Task DeleteAsync(string aegisId)
    {
        await _js.InvokeVoidAsync("aegisCrypto.deleteContact", aegisId);
    }

    public async Task<string> GetColourAsync(string aegisId)
    {
        return await _js.InvokeAsync<string>("aegisCrypto.colourForId", aegisId);
    }

    public async Task<bool> ExistsAsync(string aegisId)
    {
        var contacts = await _js.InvokeAsync<List<Contact>>("aegisCrypto.loadContacts");
        return contacts.Any(c => c.AegisId == aegisId);
    }
}
