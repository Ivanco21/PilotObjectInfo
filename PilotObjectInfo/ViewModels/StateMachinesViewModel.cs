using Ascon.Pilot.SDK;
using PilotObjectInfo.Models;
using PilotObjectInfo.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace PilotObjectInfo.ViewModels
{
    class StateMachinesViewModel : CommonViewModel
    {
        private readonly IReadOnlyDictionary<Guid, IUserState> _statesById;

        public StateMachinesViewModel(IEnumerable<IUserStateMachine> machines, IEnumerable<IUserState> states)
        {
            _statesById = (states ?? Enumerable.Empty<IUserState>())
                .Where(s => s != null)
                .GroupBy(s => s.Id)
                .ToDictionary(g => g.Key, g => g.First());

            StateMachines = new ObservableCollection<IUserStateMachine>(
                (machines ?? Enumerable.Empty<IUserStateMachine>()).Where(m => m != null));

            StateMachinesView = CollectionViewSource.GetDefaultView(StateMachines);
            StateMachinesView.Filter = FilterStateMachines;

            SearchCommand = new RelayCommand(_ => StateMachinesView.Refresh());
            SelectUserStateCommand = new RelayCommand(OnSelectUserStateExecuted, CanSelectUserStateExecute);

            if (StateMachines.Count > 0)
                SelectedStateMachine = StateMachines[0];
        }

        public ObservableCollection<IUserStateMachine> StateMachines { get; }

        #region Поиск - View
        private ICollectionView _stateMachinesView;
        public ICollectionView StateMachinesView
        {
            get => _stateMachinesView;
            set => Set(ref _stateMachinesView, value);
        }
        #endregion

        #region Поиск - значение
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                Set(ref _searchTerm, value);
                StateMachinesView.Refresh();
            }
        }
        #endregion

        #region Команда для поиска
        public ICommand SearchCommand { get; }
        #endregion

        #region Команда выбора UserState (DoubleClick по ноде)
        public ICommand SelectUserStateCommand { get; }

        private void OnSelectUserStateExecuted(object obj)
        {
            Guid? stateId = obj switch
            {
                Guid id => id,
                StateMachineVertex vertex => vertex.Id,
                IUserState state => state.Id,
                _ => null
            };

            if (stateId == null)
            {
                ClearUserStateSelection();
                return;
            }

            _statesById.TryGetValue(stateId.Value, out var userState);
            SelectedUserState = userState;
            ApplyVertexSelection(stateId);
        }

        private bool CanSelectUserStateExecute(object obj) =>
            obj is Guid or StateMachineVertex or IUserState;

        public void ClearUserStateSelection()
        {
            SelectedUserState = null;
            ApplyVertexSelection(null);
        }
        #endregion

        #region Выбранная машина состояний
        private IUserStateMachine _selectedStateMachine;
        public IUserStateMachine SelectedStateMachine
        {
            get => _selectedStateMachine;
            set
            {
                if (!Set(ref _selectedStateMachine, value))
                    return;

                ClearUserStateSelection();
                Graph = BuildGraph(value);
            }
        }
        #endregion

        #region Выбранный UserState
        private IUserState _selectedUserState;
        public IUserState SelectedUserState
        {
            get => _selectedUserState;
            set
            {
                if (!Set(ref _selectedUserState, value))
                    return;

                SelectedUserStateItems.Clear();
                if (value != null)
                    SelectedUserStateItems.Add(value);
            }
        }

        public ObservableCollection<IUserState> SelectedUserStateItems { get; } = new();
        #endregion

        #region Граф
        private StateMachineGraph _graph;
        public StateMachineGraph Graph
        {
            get => _graph;
            private set => Set(ref _graph, value);
        }
        #endregion

        private void ApplyVertexSelection(Guid? selectedStateId)
        {
            if (Graph == null)
                return;

            foreach (var vertex in Graph.Vertices)
                vertex.IsSelected = selectedStateId != null && vertex.Id == selectedStateId.Value;

            foreach (var edge in Graph.Edges)
            {
                edge.IsSelected = selectedStateId != null &&
                                 (edge.Source.Id == selectedStateId.Value ||
                                  edge.Target.Id == selectedStateId.Value);
            }
        }

        private bool FilterStateMachines(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;

            if (obj is not IUserStateMachine machine)
                return false;

            string search = SearchTerm.Trim().ToLower();
            return machine.Id.ToString().Equals(search, StringComparison.OrdinalIgnoreCase) ||
                   (!string.IsNullOrEmpty(machine.Title) && machine.Title.ToLower().Contains(search));
        }

        private StateMachineGraph BuildGraph(IUserStateMachine machine)
        {
            var graph = new StateMachineGraph();
            if (machine?.StateTransitions == null)
                return graph;

            var vertices = new Dictionary<Guid, StateMachineVertex>();

            StateMachineVertex GetOrAdd(Guid stateId)
            {
                if (vertices.TryGetValue(stateId, out var existing))
                    return existing;

                _statesById.TryGetValue(stateId, out var state);
                var vertex = new StateMachineVertex(
                    stateId,
                    state?.Name ?? stateId.ToString("D"),
                    state?.Title ?? stateId.ToString("D"));

                vertices[stateId] = vertex;
                graph.AddVertex(vertex);
                return vertex;
            }

            // Collect transitions first, then merge A↔B into one bidirectional edge
            // so GraphShape draws a single line with arrows on both ends.
            var pending = new List<(StateMachineVertex From, StateMachineVertex To, string Name)>();

            foreach (var pair in machine.StateTransitions)
            {
                var from = GetOrAdd(pair.Key);
                if (pair.Value == null)
                    continue;

                foreach (var transition in pair.Value.Where(t => t != null))
                {
                    var to = GetOrAdd(transition.StateTo);
                    pending.Add((from, to, transition.DisplayName ?? string.Empty));
                }
            }

            foreach (var group in pending.GroupBy(t => EdgePairKey(t.From.Id, t.To.Id)))
            {
                var items = group.ToList();
                var first = items[0];

                if (first.From.Id == first.To.Id)
                {
                    foreach (var item in items)
                        graph.AddEdge(new StateMachineEdge(item.From, item.To, item.Name));
                    continue;
                }

                var forward = items.Where(t => t.From.Id == first.From.Id).ToList();
                var reverse = items.Where(t => t.From.Id != first.From.Id).ToList();

                bool isBidirectional = reverse.Count > 0;
                string displayName = isBidirectional
                    ? CombineEdgeNames(JoinNames(forward.Select(t => t.Name)), JoinNames(reverse.Select(t => t.Name)))
                    : JoinNames(forward.Select(t => t.Name));

                var edgeFrom = forward[0].From;
                var edgeTo = forward[0].To;
                graph.AddEdge(new StateMachineEdge(edgeFrom, edgeTo, displayName, isBidirectional));
            }

            return graph;
        }

        private static (Guid A, Guid B) EdgePairKey(Guid from, Guid to) =>
            from.CompareTo(to) <= 0 ? (from, to) : (to, from);

        private static string JoinNames(IEnumerable<string> names)
        {
            var unique = names
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.Ordinal)
                .ToList();
            return unique.Count == 0 ? string.Empty : string.Join(", ", unique);
        }

        private static string CombineEdgeNames(string forward, string reverse)
        {
            bool hasForward = !string.IsNullOrWhiteSpace(forward);
            bool hasReverse = !string.IsNullOrWhiteSpace(reverse);

            if (hasForward && hasReverse)
            {
                if (string.Equals(forward, reverse, StringComparison.Ordinal))
                    return forward;
                return forward + " ↔ " + reverse;
            }

            if (hasForward)
                return forward;
            if (hasReverse)
                return reverse;
            return string.Empty;
        }
    }
}
