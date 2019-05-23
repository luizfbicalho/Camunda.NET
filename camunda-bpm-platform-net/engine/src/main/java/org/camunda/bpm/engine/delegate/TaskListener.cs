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
namespace org.camunda.bpm.engine.@delegate
{
	/// <summary>
	/// Listener interface implemented by user code which wants to be notified when a property of a task changes.
	/// 
	/// <para>The following Task Events are supported:
	/// <ul>
	/// <li><seealso cref="EVENTNAME_CREATE"/></li>
	/// <li><seealso cref="EVENTNAME_ASSIGNMENT"/></li>
	/// <li><seealso cref="EVENTNAME_COMPLETE"/></li>
	/// <li><seealso cref="EVENTNAME_DELETE"/></li>
	/// </ul>
	/// </para>
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface TaskListener
	{

	  void notify(DelegateTask delegateTask);

	}

	public static class TaskListener_Fields
	{
	  public const string EVENTNAME_CREATE = "create";
	  public const string EVENTNAME_ASSIGNMENT = "assignment";
	  public const string EVENTNAME_COMPLETE = "complete";
	  public const string EVENTNAME_DELETE = "delete";
	}

}