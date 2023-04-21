namespace LXNavi;

public enum EmojiType
{
    Infomation,
    Question,
    Warning,
    Error
}

public static class Emoji
{
    #region Public Methods

    public static string GetEmoji(EmojiType emojiType)
    {
        return emojiType switch
        {
            EmojiType.Infomation => "(｀・ω・´)",
            EmojiType.Question => "⊙(・◇・)？",
            EmojiType.Warning => "(╬￣皿￣)=○",
            EmojiType.Error => "┌(。Д。)┐",
            _ => string.Empty,
        };
    }

    #endregion Public Methods
}
