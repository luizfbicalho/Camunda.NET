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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.FORM_PROPERTY_UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.VARIABLE_INSTANCE_CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.VARIABLE_INSTANCE_DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.VARIABLE_INSTANCE_UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes.VARIABLE_INSTANCE_MIGRATE;

	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HistoryLevelAudit : HistoryLevelActivity
	{

	  public override int Id
	  {
		  get
		  {
			return 2;
		  }
	  }

	  public override string Name
	  {
		  get
		  {
			return ProcessEngineConfiguration.HISTORY_AUDIT;
		  }
	  }

	  public override bool isHistoryEventProduced(HistoryEventType eventType, object entity)
	  {
		return base.isHistoryEventProduced(eventType, entity) || VARIABLE_INSTANCE_CREATE == eventType || VARIABLE_INSTANCE_UPDATE == eventType || VARIABLE_INSTANCE_MIGRATE == eventType || VARIABLE_INSTANCE_DELETE == eventType || FORM_PROPERTY_UPDATE == eventType;

	  }

	}

}