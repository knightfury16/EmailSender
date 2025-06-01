using EmailSenderLib.Models;

namespace EmailSenderLib.TemplateRenderer;

public interface ITemplateRenderer
{
    /// <summary>
    /// Renders the specified template using provided data.
    /// </summary>
    /// <param name="templateId">Identifier of the template to render.</param>
    /// <param name="data">Data used during template rendering.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The rendered template content.</returns>
    public Task<RenderedContent> RenderTemplateAsync(
        object data,
        string? templateId,
        CancellationToken cancellationToken = default
    );

    public Task<RenderedContent> RenderTemplateAsync(
        object data,
        CancellationToken cancellationToken = default
    );

    public Task<RenderedContent> RenderTemplateAsync(
        string templateId,
        CancellationToken cancellationToken = default
    );
}
