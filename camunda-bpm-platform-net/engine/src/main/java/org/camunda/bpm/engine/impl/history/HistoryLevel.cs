/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.history
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;

	/// <summary>
	/// <para>The history level controls what kind of data is logged to the history database.
	/// More formally, it controls which history events are produced by the <seealso cref="HistoryEventProducer"/>.</para>
	/// 
	/// <para><strong>Built-in history levels:</strong> The process engine provides a set of built-in history levels
	/// as default configuration. The built-in history levels are:
	/// <ul>
	///   <li><seealso cref="#HISTORY_LEVEL_NONE"/></li>
	///   <li><seealso cref="#HISTORY_LEVEL_ACTIVITY"/></li>
	///   <li><seealso cref="#HISTORY_LEVEL_AUDIT"/></li>
	///   <li><seealso cref="#HISTORY_LEVEL_FULL"/></li>
	/// </ul>
	/// This class provides singleton instances of these history levels as constants.
	/// </para>
	/// 
	/// <para><strong>Custom history levels:</strong>In order to implement a custom history level,
	/// the following steps are necessary:
	/// <ul>
	///   <li>Provide a custom implementation of this interface. Note: Make sure you choose unique values for
	///   <seealso cref="#getName()"/> and <seealso cref="#getId()"/></li>
	///   <li>Add an instance of the custom implementation through
	///   <seealso cref="ProcessEngineConfigurationImpl#setCustomHistoryLevels(java.util.List)"/></li>
	///   <li>use the name of your history level (as returned by <seealso cref="#getName()"/> as value for
	///   <seealso cref="ProcessEngineConfiguration#setHistory(String)"/></li>
	/// </ul>
	/// </para>
	/// 
	/// @author Daniel Meyer
	/// @since 7.2
	/// </summary>
	public interface HistoryLevel
	{

	  /// <summary>
	  /// An unique id identifying the history level.
	  /// The id is used internally to uniquely identify the history level and also stored in the database.
	  /// </summary>
	  int Id {get;}

	  /// <summary>
	  /// An unique name identifying the history level.
	  /// The name of the history level can be used when configuring the process engine. </summary>
	  /// <seealso cref= <seealso cref="ProcessEngineConfiguration#setHistory(String)"/> </seealso>
	  string Name {get;}

	  /// <summary>
	  /// Returns true if a given history event should be produced. </summary>
	  /// <param name="eventType"> the type of the history event which is about to be produced </param>
	  /// <param name="entity"> the runtime structure used to produce the history event. Examples <seealso cref="ExecutionEntity"/>,
	  /// <seealso cref="TaskEntity"/>, <seealso cref="VariableInstanceEntity"/>, ... If a 'null' value is provided, the implementation
	  /// should return true if events of this type should be produced "in general". </param>
	  bool isHistoryEventProduced(HistoryEventType eventType, object entity);

	}

	public static class HistoryLevel_Fields
	{
	  public static readonly HistoryLevel HISTORY_LEVEL_NONE = new HistoryLevelNone();
	  public static readonly HistoryLevel HISTORY_LEVEL_ACTIVITY = new HistoryLevelActivity();
	  public static readonly HistoryLevel HISTORY_LEVEL_AUDIT = new HistoryLevelAudit();
	  public static readonly HistoryLevel HISTORY_LEVEL_FULL = new HistoryLevelFull();
	}

}