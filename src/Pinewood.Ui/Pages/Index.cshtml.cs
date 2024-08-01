using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pinewood.Ui.Models;

namespace Pinewood.Ui.Pages;

public class IndexModel(HttpClient httpClient) : PageModel
{
    private const int PageSize = 10;

    public List<Customer> Customers { get; set; } = [];
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => Customers.Count == PageSize;

    public async Task OnGetAsync() => await LoadCustomersAsync();

    public async Task<IActionResult> OnPostCreateAsync(CreateCustomerCommand command)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/customers", command);
        return response.IsSuccessStatusCode ? RedirectToPage() : Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"/api/customers/{id}");
        return response.IsSuccessStatusCode ? RedirectToPage() : Page();
    }
    
    private async Task LoadCustomersAsync()
    {
        var query = new GetCustomerQuery(CurrentPage, PageSize);
        Customers = await httpClient.GetFromJsonAsync<List<Customer>>($"/api/customers?PageNumber={query.PageNumber}&PageSize={query.PageSize}") ?? [];
    }
}