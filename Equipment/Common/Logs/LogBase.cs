using LogParser.Common.Structs;
using LogParser.Equipment.Common.Enums;
using LogParser.Equipment.Common.Messages;
using LogParser.Subsystems.Interface;
using LogParser.Subsystems.Tracking;
using LogParser.System;

namespace LogParser.Equipment.Common.Parsers
{
    internal abstract class LogBase<TModel, TSubsystem>(
            TSubsystem trackedSubsystem,
            Dictionary<MessageClass, bool> enabledMessages,
            LogReaderConsole? console = null)
        where TModel : ISubsystemModel
        where TSubsystem : TrackedSubsystem<TModel>
    {
        #region Fields

        protected readonly LogReaderConsole? _console = console;

        protected Dictionary<MessageClass, bool> _enabledMessages = enabledMessages;

        protected readonly List<TModel> _inboundHistory = [];

        #endregion

        #region Properties

        public DateTimeOffset? LogTimeStamp { get; set; }

        public TSubsystem TrackedSubsystem { get; } = trackedSubsystem;

        protected readonly Queue<ProcessMessage> MessageProcessors = [];

        #endregion

        #region Delegates

        protected delegate IParseableMessage? ProcessMessage(string input, int lineNumber, ref bool wasParsed);

        #endregion

        #region Methods

        public abstract void ProcessLine(string input, int lineNumber);

        protected Timestamp GetTimeStamp(TimeOnly time, int lineNumber) =>
            new(new DateTimeOffset(
                    year: LogTimeStamp?.Year ?? throw new NullReferenceException($"{LogTimeStamp} must be initialized before use."),
                    month: LogTimeStamp?.Month ?? throw new NullReferenceException($"{LogTimeStamp} must be initialized before use."),
                    day: LogTimeStamp?.Day ?? throw new NullReferenceException($"{LogTimeStamp} must be initialized before use."),
                    hour: time.Hour,
                    minute: time.Minute,
                    second: time.Second,
                    millisecond: time.Millisecond,
                    offset: LogTimeStamp?.Offset ?? throw new NullReferenceException($"{LogTimeStamp} must be initialized before use.")),
                lineNumber);

        #endregion
    }
}