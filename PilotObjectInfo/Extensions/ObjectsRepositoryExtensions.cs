using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.Extensions
{
    //TODO: Decompiled! not refactored
    public static class ObjectsRepositoryExtensions
    {
        public static Task<IEnumerable<T>> GetObjectsAsync<T>(
          this IObjectsRepository repository,
          IEnumerable<Guid> ids,
          Func<IDataObject, T> converter,
          CancellationToken ct)
        {
            IObservable<T> other = Observable.Create<T>((Func<IObserver<T>, IDisposable>)(o => (IDisposable)ct.Register(new Action(o.OnCompleted))));
            List<Guid> list = ids.ToList();
            IObservable<IList<T>> observableList = repository.SubscribeObjects((IEnumerable<Guid>)list).TakeUntil<IDataObject, T>(other).Where<IDataObject>((Func<IDataObject, bool>)(o => o.State == DataState.Loaded)).Distinct<IDataObject, Guid>((Func<IDataObject, Guid>)(o => o.Id)).Select<IDataObject, T>(converter).Take<T>(list.Count).ToList<T>();
            return Task<IEnumerable<T>>.Factory.StartNew((Func<IEnumerable<T>>)(() => (IEnumerable<T>)observableList.Wait<IList<T>>().ToList<T>()), ct);
        }

        public static Task<IEnumerable<T>> GetObjectsAsync<T>(
          this IObjectsRepository repository,
          IEnumerable<Guid> ids,
          Func<IDataObject, T> converter,
          CancellationToken ct,
          DispatcherPriority dp)
        {
            IObservable<T> other = Observable.Create<T>((Func<IObserver<T>, IDisposable>)(o => (IDisposable)ct.Register(new Action(o.OnCompleted))));
            List<Guid> list = ids.ToList();
            IObservable<IList<T>> observableList = repository.SubscribeObjects((IEnumerable<Guid>)list).TakeUntil<IDataObject, T>(other).ObserveOnDispatcher<IDataObject>(dp).Where<IDataObject>((Func<IDataObject, bool>)(o => o.State == DataState.Loaded)).Distinct<IDataObject, Guid>((Func<IDataObject, Guid>)(o => o.Id)).Select<IDataObject, T>(converter).Take<T>(list.Count).ToList<T>();
            return Task<IEnumerable<T>>.Factory.StartNew((Func<IEnumerable<T>>)(() => (IEnumerable<T>)observableList.Wait<IList<T>>().ToList<T>()), ct);
        }
    }
}
