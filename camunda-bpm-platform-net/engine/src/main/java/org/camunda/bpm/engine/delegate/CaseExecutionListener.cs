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
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;

	/// <summary>
	/// Listener interface implemented by user code which wants to be notified
	/// when a state transition happens on a <seealso cref="CaseExecution"/>.
	/// 
	/// <para>The following state transition are supported on a <seealso cref="CaseInstance"/>:
	/// <ul>
	/// <li><seealso cref="#CREATE"/></li>
	/// <li><seealso cref="#COMPLETE"/></li>
	/// <li><seealso cref="#TERMINATE"/></li>
	/// <li><seealso cref="#SUSPEND"/></li>
	/// <li><seealso cref="#RE_ACTIVATE"/></li>
	/// <li><seealso cref="#CLOSE"/></li>
	/// </ul>
	/// </para>
	/// 
	/// <para>And on a <seealso cref="CaseExecution"/> which is not a <seealso cref="CaseInstance"/> and which
	/// is associated with a <seealso cref="Task"/> or a <seealso cref="Stage"/> the following state transition
	/// are supported:
	/// <ul>
	/// <li><seealso cref="#CREATE"/></li>
	/// <li><seealso cref="#ENABLE"/></li>
	/// <li><seealso cref="#DISABLE"/></li>
	/// <li><seealso cref="#RE_ENABLE"/></li>
	/// <li><seealso cref="#START"/></li>
	/// <li><seealso cref="#MANUAL_START"/></li>
	/// <li><seealso cref="#COMPLETE"/></li>
	/// <li><seealso cref="#TERMINATE"/></li>
	/// <li><seealso cref="#EXIT"/></li>
	/// <li><seealso cref="#SUSPEND"/></li>
	/// <li><seealso cref="#RESUME"/></li>
	/// <li><seealso cref="#PARENT_SUSPEND"/></li>
	/// <li><seealso cref="#PARENT_RESUME"/></li>
	/// </ul>
	/// </para>
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CaseExecutionListener : DelegateListener<DelegateCaseExecution>
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void notify(DelegateCaseExecution caseExecution) throws Exception;
	  void notify(DelegateCaseExecution caseExecution);

	}

	public static class CaseExecutionListener_Fields
	{
	  public const string CREATE = "create";
	  public const string ENABLE = "enable";
	  public const string DISABLE = "disable";
	  public const string RE_ENABLE = "reenable";
	  public const string START = "start";
	  public const string MANUAL_START = "manualStart";
	  public const string COMPLETE = "complete";
	  public const string RE_ACTIVATE = "reactivate";
	  public const string TERMINATE = "terminate";
	  public const string EXIT = "exit";
	  public const string PARENT_TERMINATE = "parentTerminate";
	  public const string SUSPEND = "suspend";
	  public const string RESUME = "resume";
	  public const string PARENT_SUSPEND = "parentSuspend";
	  public const string PARENT_RESUME = "parentResume";
	  public const string CLOSE = "close";
	  public const string OCCUR = "occur";
	}

}