using Content.Shared.Access.Systems;
using Content.Shared.MedicalRecords;
using Content.Shared.MedicalRecords.Components;
using Content.Shared.Medical;
using Content.Shared.StationRecords;
using Robust.Client.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Console;


namespace Content.Client.MedicalRecords;

public sealed class MedicalRecordsConsoleBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    private readonly AccessReaderSystem _accessReader;

    private MedicalRecordsConsoleWindow? _window;
    private MedicalHistoryWindow? _historyWindow;

    public MedicalRecordsConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _accessReader = EntMan.System<AccessReaderSystem>();
    }

    protected override void Open()
    {
        base.Open();

        var comp = EntMan.GetComponent<MedicalRecordsConsoleComponent>(Owner);

        _window = new(Owner, comp.MaxStringLength, _playerManager, _proto, _random, _accessReader);
        _window.OnKeySelected += key =>
            SendMessage(new SelectStationRecord(key));
        _window.OnFiltersChanged += (type, filterValue) =>
            SendMessage(new SetStationRecordFilter(type, filterValue));
        _window.OnStatusSelected += status =>
            SendMessage(new MedicalRecordChangeStatus(status, null));
        _window.OnDialogConfirmed += (status, reason) =>
            SendMessage(new MedicalRecordChangeStatus(status, reason));
        _window.OnHistoryUpdated += UpdateHistory;
        _window.OnHistoryClosed += () => _historyWindow?.Close();
        _window.OnPrintButtonPressed += key =>
            SendMessage(new PrintMedicalCard(key));
        _window.OnClose += Close;

        _historyWindow = new(comp.MaxStringLength);
        _historyWindow.OnAddHistory += line => SendMessage(new MedicalRecordAddHistory(line));
        _historyWindow.OnDeleteHistory += index => SendMessage(new MedicalRecordDeleteHistory(index));

        _historyWindow.Close(); // leave closed until user opens it
    }

    /// <summary>
    /// Updates or opens a new history window.
    /// </summary>
    private void UpdateHistory(MedicalRecord record, bool access, bool open)
    {
        _historyWindow!.UpdateHistory(record, access);

        if (open)
            _historyWindow.OpenCentered();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not MedicalRecordsConsoleState cast)
            return;

        _window?.UpdateState(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _window?.Close();
        _historyWindow?.Close();
    }
}
