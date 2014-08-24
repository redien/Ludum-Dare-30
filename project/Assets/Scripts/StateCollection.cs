using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

[Serializable()]
public class StateCollection
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
		public bool propegateStateChanges;
		public VisibleTo visibleTo = VisibleTo.All;
		public int prefab;
		public float disableAfter;
	}

	State[] states;
	
	public int level = 0;
	
	public int Count {
		get {
			return states.Length;
		}
	}

	void SetStateInternal(int stateId, bool state) {
		states[stateId].enabled = state;
	}
	
	public delegate void StateChanged(int stateId, bool state, bool broadcast = true);
	[field: NonSerialized]
	public event StateChanged stateChanged;
	
	public void SetState(int stateId, bool state, bool broadcast = true) {
		if (!states[stateId].enabled && state) {
			if (stateChanged != null) {
				stateChanged(stateId, state, broadcast);
			}
			StateWasEnabled(stateId);
		}
		
		if (states[stateId].enabled && !state) {
			if (stateChanged != null) {
				stateChanged(stateId, state, broadcast);
			}
		}

		SetStateInternal(stateId, state);
	}

	public bool GetState(int stateId) {
		return states[stateId].enabled;
	}
	
	void StateWasEnabled(int stateId) {
		if (states[stateId].statesToEnableOnEnable != null) {
			foreach (var stateIdToEnable in states[stateId].statesToEnableOnEnable) {
				if (states[stateId].propegateStateChanges) {
					SetState(stateIdToEnable, true, false);
				} else {
					SetStateInternal(stateIdToEnable, true);
				}
			}
		}

		if (states[stateId].statesToDisableOnEnable != null) {
			foreach (var stateIdToDisable in states[stateId].statesToDisableOnEnable) {
				if (states[stateId].propegateStateChanges) {
					SetState(stateIdToDisable, false, false);
				} else {
					SetStateInternal(stateIdToDisable, false);
				}
			}
		}
	}
	
	public int GetPrefabOf(int stateId) {
		return states[stateId].prefab;
	}
	
	public State GetStateSettings(int stateId) {
		return states[stateId];
	}
	
	public void Generate() {
		List<State> generatedStates = new List<State>();
		
		switch (level) {
		case 4:
			{
				State state = new State();
				generatedStates.Add(state);
			}
		break;
			
		case 1:
			{
				State state = new State();
				generatedStates.Add(state);
			}
			{
				State state = new State();
				generatedStates.Add(state);
			}
		break;
			
		case 2:
			{
				State state = new State();
				generatedStates.Add(state);
			}
			{
				State state = new State();
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.prefab = 1;
				generatedStates.Add(state);
			}
		break;

		case 3:
			{
				State state = new State();
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.prefab = 1;
				state.disableAfter = 3.0f;
				generatedStates.Add(state);
			}
		break;

		case 0:
			{
				State state = new State();
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.statesToDisableOnEnable = new int[1];
				state.statesToDisableOnEnable[0] = 0;
				state.prefab = 1;
				generatedStates.Add(state);
			}
		break;

		case 5:
			{
				State state = new State();
				state.statesToDisableOnEnable = new int[1];
				state.statesToDisableOnEnable[0] = 1;
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.statesToDisableOnEnable = new int[1];
				state.statesToDisableOnEnable[0] = 0;
				state.prefab = 1;
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.statesToEnableOnEnable = new int[2];
				state.statesToEnableOnEnable[0] = 0;
				state.statesToEnableOnEnable[1] = 1;
				state.prefab = 1;
				generatedStates.Add(state);
			}
		break;

		case 6:
			{
				State state = new State();
				state.statesToEnableOnEnable = new int[1];
				state.statesToEnableOnEnable[0] = 1;
				state.propegateStateChanges = true;
				state.disableAfter = 3.0f;
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.statesToEnableOnEnable = new int[1];
				state.statesToEnableOnEnable[0] = 2;
				state.propegateStateChanges = true;
				state.disableAfter = 3.0f;
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.statesToEnableOnEnable = new int[1];
				state.statesToEnableOnEnable[0] = 3;
				state.propegateStateChanges = true;
				state.disableAfter = 3.0f;
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.disableAfter = 3.0f;
				generatedStates.Add(state);
			}
		break;

		case 7:
			{
				State state = new State();
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.prefab = 1;
				generatedStates.Add(state);
			}
			{
				State state = new State();
				state.prefab = 1;
				state.visibleTo = State.VisibleTo.One;
				generatedStates.Add(state);
			}
		break;

		default: {
		}
		break;
		}
		
		states = generatedStates.ToArray();
	}

	public static StateCollection Deserialize(string serialized) {
		BinaryFormatter deserializer = new BinaryFormatter();
		
		using (MemoryStream stream = new MemoryStream()) {
			var converted = Convert.FromBase64String(serialized);
			stream.Write(converted, 0, converted.Length);
			stream.Seek(0, SeekOrigin.Begin);
	
	        return (StateCollection)deserializer.Deserialize(stream);
		}
	}
	
	public static string Serialize(StateCollection stateCollection) {
		using (MemoryStream stream = new MemoryStream()) {
			BinaryFormatter serializer = new BinaryFormatter();
			serializer.Serialize(stream, stateCollection);
			stream.Flush();
			stream.Position = 0;
			return Convert.ToBase64String(stream.ToArray());
		}
	}
}
