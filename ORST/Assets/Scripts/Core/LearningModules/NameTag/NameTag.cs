using ORST.Core.Placeholders;

namespace ORST.Core.LearningModules {
    public static class NameTag {
        private static NameTagKind s_NameTagKind;
        public static NameTagKind Kind {
            get => s_NameTagKind;
            internal set {
                s_NameTagKind = value;
                PlaceholderManager.AddPlaceholder("{CORRECT COMPANY}", s_NameTagKind == NameTagKind.DePuy ? "J&J DePuy Synthes" : "J&J Ethicon");
                PlaceholderManager.AddPlaceholder("{WRONG COMPANY}", s_NameTagKind == NameTagKind.DePuy ? "J&J Ethicon" : "J&J DePuy Synthes");
            }
        }
    }
}