using System;
using System.Collections.Generic;

public static class Postbox
{
	public partial class Key_Internal
	{
		public MessageTopics Topic { get; set; }
		public Type PayloadType { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			return obj is Key_Internal otherKey
			? otherKey.Topic.Equals(Topic) && otherKey.PayloadType.Equals(PayloadType)
			: false;
		}

		public override int GetHashCode() => HashCode.Combine(Topic, PayloadType);
	}

	public partial class Value_Internal
	{
		public int Handle { get; set; }
		public Action<object> Callback { get; set; }
	}

	// Main reason for this class is to avoid having to go through
	// every entry inside the dictionary since the keys are made out of
	// topics and the types
	public partial class Subscriber
	{
		private Key_Internal _storedKey;
		private int _handle;

		static Subscriber()
		{
			// Yoinked from
			// https://stackoverflow.com/questions/1664793/how-to-restrict-access-to-nested-class-member-to-enclosing-class
			HiddenSubscriberConstructor = (x, y) => new Subscriber(x, y);
		}

		private Subscriber(Key_Internal key, int handle)
		{
			_storedKey = key;
			_handle = handle;
		}

		public void Unsubscribe()
		{
			_subscribers[_storedKey].RemoveAll(x => x.Handle == _handle);
		}
	}

	private static Dictionary<Key_Internal, List<Value_Internal>> _subscribers =
		new Dictionary<Key_Internal, List<Value_Internal>>();
	private static int _handleCounter = -1;
	private static Func<Key_Internal, int, Subscriber> HiddenSubscriberConstructor;

	public static void Publish<T>(MessageTopics topic, T payload)
	{
		// This should allow for different payloads to be reached on the same topic.
		var key = new Key_Internal() { Topic = topic, PayloadType = typeof(T) };
		if (_subscribers.ContainsKey(key))
		{
			_subscribers[key].ForEach(x => x.Callback(payload));
		}
	}

	public static Subscriber Subscribe<T>(MessageTopics topic, Action<T> callback)
	{
		var key = new Key_Internal() { Topic = topic, PayloadType = typeof(T) };
		// Wraps the callback into a lambda and stores that instead.
		// Yoinked from https://stackoverflow.com/questions/3444246/convert-actiont-to-actionobject
		var value = new Value_Internal()
		{
			Handle = ++_handleCounter,
			Callback = (Action<object>)(o => callback((T)o))
		};
		if (!_subscribers.ContainsKey(key))
		{
			_subscribers.Add(key, new List<Value_Internal>() { value });
		}
		else
		{
			_subscribers[key].Add(value);
		}
		if (HiddenSubscriberConstructor is null)
		{
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Subscriber).TypeHandle);
		}
		return HiddenSubscriberConstructor(key, value.Handle);
	}
}

public enum MessageTopics
{
	DamageDealt,
}
