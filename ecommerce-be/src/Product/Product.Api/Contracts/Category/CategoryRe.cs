namespace Product.Api.Contracts.Category;

public record CategoryCreateRequest(string Name, String Slug, String? Description);
public record CategoryCreateResponse(Guid Id, string Name, String Slug, String? Description);

public record CategoryUpdateRequest(Guid Id, string Name, String Slug, String? Description);
public record CategoryUpdateResponse(Guid Id, string Name, String Slug, String? Description);