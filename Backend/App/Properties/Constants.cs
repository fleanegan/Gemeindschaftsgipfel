namespace Kompetenzgipfel.Properties;

public static class Constants
{
    public const int MaxLengthTitle = 50;
    public const string MaxLengthTitleErrorMessage = "Vortragstitel dürfen maximal {0} Zeichen lang sein.";
    public const string EmptyTitleErrorMessage = "Ein Vortrag braucht einen Titel";

    public const string WrongPassphraseErrorMessage =
        "Falsches Eintrittsgeheimnis. Stell sicher, dass du den richtigen Satz aus der Einladungsnachricht kopierst";

    public const int MaxLengthDescription = 10000;

    public const string MaxLengthDescriptionErrorMessage =
        "Vortragsbeschreibungen dürfen maximal {0} Zeichen lang sein.";

    public const string EmptyIdErrorMessage =
        "Die Vortragsidentitifikationsnummer ist obligatorisch, um Vortragsthemen zu modifizieren.";
}