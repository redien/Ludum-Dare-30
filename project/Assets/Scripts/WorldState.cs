using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable()]
public class WorldState
{
	[Serializable()]
	public class State {
		public enum VisibleTo {
			All,
			One
		}
		
		public bool enabled;
		public int[] statesToEnableOnEnable;
		public int[] statesToDisableOnEnable;
		public VisibleTo visibleTo;
		public int prefab;
	}

	public State[] states;
	
	public int Count {
		get {
			return states.Length;
		}
	}
	
	public WorldState ()
	{
	}
	
	void SetStateInternal(int stateId, bool state) {
		states[stateId].enabled = state;
	}
	
	public void SetState(int stateId, bool state) {
		SetStateInternal(stateId, state);
		
		if (!states[stateId].enabled && state) {
			StateWasEnabled(stateId);
		}
	}
	
	public bool GetState(int stateId) {
		return states[stateId].enabled;
	}
	
	void StateWasEnabled(int stateId) {
		if (states[stateId].statesToEnableOnEnable != null) {
			foreach (var stateIdToEnable in states[stateId].statesToEnableOnEnable) {
				SetStateInternal(stateIdToEnable, true);
			}
		}

		if (states[stateId].statesToDisableOnEnable != null) {
			foreach (var stateIdToDisable in states[stateId].statesToDisableOnEnable) {
				SetStateInternal(stateIdToDisable, false);
			}
		}
	}
	
	public int GetPrefabOf(int stateId) {
		return states[stateId].prefab;
	}
	
	public void Generate(int level) {
		states = new State[1];
		
		State state = new State();
		state.visibleTo = State.VisibleTo.All;
		states[0] = state;
	}
	
	public static WorldState Deserialize(string serialized) {
		BinaryFormatter deserializer = new BinaryFormatter();
		
		using (MemoryStream stream = new MemoryStream()) {
			var converted = Convert.FromBase64String(serialized);
			stream.Write(converted, 0, converted.Length);
			stream.Seek(0, SeekOrigin.Begin);
	
	        return (WorldState)deserializer.Deserialize(stream);
		}
	}
	
	public static string Serialize(WorldState worldState) {
		using (MemoryStream stream = new MemoryStream()) {
			BinaryFormatter serializer = new BinaryFormatter();
			serializer.Serialize(stream, worldState);
			stream.Flush();
			stream.Position = 0;
			return Convert.ToBase64String(stream.ToArray());
		}
	}
}
