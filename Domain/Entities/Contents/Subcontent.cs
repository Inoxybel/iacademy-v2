using CrossCutting.Enums;

namespace Domain.Entities.Contents;

public class Subcontent
{
    public List<SubcontentHistory> SubcontentHistory {  get; set; }

    public SubcontentHistory FindSubcontentHistoryByGenre(TextGenre genre) =>
        SubcontentHistory.FirstOrDefault(history => history.TextGenre == genre);
}
