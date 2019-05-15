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
namespace org.camunda.bpm.engine.cdi
{
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;

	/// <summary>
	/// The type of a business process event. Indicates what is happening/has
	/// happened, i.e. whether a transition is taken, an activity is entered or left,
	/// a task is created, assigned, completed or deleted.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public interface BusinessProcessEventType
	{

	  /// <summary>
	  /// Signifies that a transition is being taken / was taken * </summary>

	  /// <summary>
	  /// Signifies that an activity is being entered / war entered * </summary>

	  /// <summary>
	  /// Signifies that an activity is being left / was left * </summary>

	  /// <summary>
	  /// Signifies that a task is created * </summary>

	  /// <summary>
	  /// Signifies that a task is assigned * </summary>

	  /// <summary>
	  /// Signifies that a task is completed * </summary>

	  /// <summary>
	  /// Signifies that a task is deleted * </summary>

	  string TypeName {get;}

	}

	public static class BusinessProcessEventType_Fields
	{
	  public static readonly BusinessProcessEventType TAKE = new BusinessProcessEventType_DefaultBusinessProcessEventType(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_TAKE);
	  public static readonly BusinessProcessEventType START_ACTIVITY = new BusinessProcessEventType_DefaultBusinessProcessEventType(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START);
	  public static readonly BusinessProcessEventType END_ACTIVITY = new BusinessProcessEventType_DefaultBusinessProcessEventType(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END);
	  public static readonly BusinessProcessEventType CREATE_TASK = new BusinessProcessEventType_DefaultBusinessProcessEventType(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE);
	  public static readonly BusinessProcessEventType ASSIGN_TASK = new BusinessProcessEventType_DefaultBusinessProcessEventType(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT);
	  public static readonly BusinessProcessEventType COMPLETE_TASK = new BusinessProcessEventType_DefaultBusinessProcessEventType(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE);
	  public static readonly BusinessProcessEventType DELETE_TASK = new BusinessProcessEventType_DefaultBusinessProcessEventType(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE);
	}

	  public class BusinessProcessEventType_DefaultBusinessProcessEventType : BusinessProcessEventType
	  {

	protected internal readonly string typeName;

	public BusinessProcessEventType_DefaultBusinessProcessEventType(string typeName)
	{
	  this.typeName = typeName;
	}

	public virtual string TypeName
	{
		get
		{
		  return typeName;
		}
	}

	public override string ToString()
	{
	  return typeName;
	}

	  }

}