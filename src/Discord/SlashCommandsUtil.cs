using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SlashCommandsUtil
{
    public static class SlashCommandHandler
    {
        public static async Task Handler(SocketSlashCommand command)
        {
            string commandName = command.Data.Name;
            SlashCommand foundCommand = Commands.List().FirstOrDefault(x => x.Name == commandName);

            if(foundCommand == null) 
            { 
                await command.RespondAsync("Command not found!");
                return;
            }

            int i=0;
            foreach(SlashCommandOption slashCommandOption in foundCommand.Options)
            {
                SocketSlashCommandDataOption option = command.Data.Options.ElementAtOrDefault(i);
                Type optionValueType = option.Value.GetType();

                if(option == null)
                {
                    await command.RespondAsync(BotCommands2.Response($"Couldn't find the value for option **{slashCommandOption.Name}**.", false)); 
                    return;
                }
                else if(optionValueType != slashCommandOption.ValueTypeErrorMessagePair.Item1)
                {
                    await command.RespondAsync(BotCommands2.Response(slashCommandOption.ValueTypeErrorMessagePair.Item2 ?? $"Option type doesn't match. **{slashCommandOption.Name}** must be type of `{slashCommandOption.ValueTypeErrorMessagePair.Item1}`, got `{optionValueType}`.", false)); 
                    return;
                }

                i++;
            }

            string result = await foundCommand.HandlerFunction(command.Data);
            await command.RespondAsync(string.IsNullOrEmpty(result) ? "Command has been executed." : result);
        }
    }

    public class SlashCommand
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Func<SocketSlashCommandData, Task<string>> HandlerFunction { get; private set; }
        public List<SlashCommandOption> Options { get; private set; }

        public SlashCommand(string name, string description, Func<SocketSlashCommandData, Task<string>> handlerFunction, List<SlashCommandOption> options=null)
        {
            Name = name;
            Description = description;
            HandlerFunction = handlerFunction;
            Options = options ?? new List<SlashCommandOption>();
        }
    }

    public class SlashCommandOption
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ApplicationCommandOptionType Type { get; private set; }
        public (Type, string) ValueTypeErrorMessagePair { get; private set; }
        public bool? IsRequired { get; private set; } = null;
        public bool? IsDefault { get; private set; } = null;
        public double? MinValue { get; private set; } = null;
        public double? MaxValue { get; private set; } = null;

        public SlashCommandOption(string name, 
                                  string description, 
                                  ApplicationCommandOptionType type, 
                                  (Type, string) valueTypeErrorMessagePair,
                                  bool? isRequired = null, 
                                  bool? isDefault = null, 
                                  double? minValue = null, 
                                  double? maxValue = null)
        {
            Name = name;
            Description = description;
            Type = type;
            ValueTypeErrorMessagePair = valueTypeErrorMessagePair;
            IsRequired = isRequired;
            IsDefault = isDefault;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
