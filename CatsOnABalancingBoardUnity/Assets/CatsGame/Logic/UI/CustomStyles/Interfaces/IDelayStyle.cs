namespace OMC.UI.CustomStyles {
    public interface IDelayStyle : IStyle {
        void ApplyDelay(int visibleCharacters, ref float cooldown);

        void ReceiveStartCharacter(int index, int value, bool broken);
    }
}
