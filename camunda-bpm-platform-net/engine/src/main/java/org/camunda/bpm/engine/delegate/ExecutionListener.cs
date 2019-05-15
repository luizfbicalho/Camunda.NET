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
	/// Callback interface to be notified of execution events like starting a process instance,
	/// ending an activity instance or taking a transition.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface ExecutionListener : DelegateListener<DelegateExecution>
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void notify(DelegateExecution execution) throws Exception;
	  void notify(DelegateExecution execution);
	}

	public static class ExecutionListener_Fields
	{
	  public const string EVENTNAME_START = "start";
	  public const string EVENTNAME_END = "end";
	  public const string EVENTNAME_TAKE = "take";
	}

}