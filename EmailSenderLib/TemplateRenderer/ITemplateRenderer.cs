using EmailSenderLib.Models;

namespace EmailSenderLib.TemplateRenderer;


public interface ITemplateRenderer
{
    public Task<RenderedContent> RenderTemplateAsync(
        string tempalteId,
        object data,
        CancellationToken cancellationToken = default
    );
}
