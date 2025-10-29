namespace TheCodeKitchen.Application.Business.Grains.KitchenManagementGrain;

public sealed partial class KitchenManagementGrain
{
    private const int MaxAttempts = 10;

    public async Task<Result<string>> GenerateUniqueCode(Guid kitchenId, int length, string characters)
    {
        var validCharacters = characters.ToCharArray();

        while (true)
        {
            for (var attempt = 0; attempt < MaxAttempts; attempt++)
            {
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                {
                    chars[i] = validCharacters[Random.Shared.Next(validCharacters.Length)];
                }

                var code = new string(chars);

                if (state.State.KitchenCodes.Keys.Contains(code)) continue;

                state.State.KitchenCodes.Add(code, kitchenId);
                await state.WriteStateAsync();
                return code;
            }

            length++;
        }
    }
}