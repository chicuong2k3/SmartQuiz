namespace SmartQuiz.Data.Models;

public class Flashcard : Entity
{
    private Flashcard()
    {
    }

    public Flashcard(Guid studySetId, string? frontText, string? backText, string? imageUrl, string? audioUrl)
    {
        StudySetId = studySetId;
        FrontText = frontText;
        BackText = backText;
        ImageUrl = imageUrl;
        AudioUrl = audioUrl;
    }

    public Guid StudySetId { get; set; }
    public string? FrontText { get; set; }
    public string? BackText { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
}