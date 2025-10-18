using System;
using System.Globalization;
using System.Windows.Data;
using MiniRPG.Models;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Multi-value converter that determines quest status indicator for NPCs.
    /// Returns "(Quest!)" if NPC has an offered quest not yet accepted by player.
    /// Returns "(Thanks!)" if player has completed the NPC's quest.
    /// Returns empty string otherwise.
    /// </summary>
    public class NPCQuestStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = NPC
            // values[1] = Player
            
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return string.Empty;
            
            if (values[0] is not NPC npc || values[1] is not Player player)
                return string.Empty;
            
            // Check if NPC has an offered quest
            if (npc.OfferedQuest == null)
                return string.Empty;
            
            // Check if player has completed this quest
            if (player.CompletedQuests.Any(q => q.Title == npc.OfferedQuest.Title))
                return " (Thanks!)";
            
            // Check if player has accepted this quest (but not completed it)
            if (player.ActiveQuests.Any(q => q.Title == npc.OfferedQuest.Title))
                return string.Empty; // Quest is in progress, no indicator
            
            // Quest not yet accepted
            return " (Quest!)";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
