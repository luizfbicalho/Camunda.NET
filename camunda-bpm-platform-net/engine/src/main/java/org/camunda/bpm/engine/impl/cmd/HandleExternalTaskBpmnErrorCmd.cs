using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl.cmd
{

	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// Command to handle an external task BPMN error.
	/// 
	/// @author Christopher Zell
	/// </summary>
	public class HandleExternalTaskBpmnErrorCmd : HandleExternalTaskCmd
	{
	  /// <summary>
	  /// The error code of the corresponding bpmn error.
	  /// </summary>
	  protected internal string errorCode;
	  protected internal string errorMessage;
	  protected internal IDictionary<string, object> variables;

	  public HandleExternalTaskBpmnErrorCmd(string externalTaskId, string workerId, string errorCode) : base(externalTaskId, workerId)
	  {
		this.errorCode = errorCode;
	  }

	  public HandleExternalTaskBpmnErrorCmd(string externalTaskId, string workerId, string errorCode, string errorMessage, IDictionary<string, object> variables) : base(externalTaskId, workerId)
	  {
		this.errorCode = errorCode;
		this.errorMessage = errorMessage;
		this.variables = variables;
	  }

	  protected internal override void validateInput()
	  {
		base.validateInput();
		EnsureUtil.ensureNotNull("errorCode", errorCode);
	  }

	  public override string ErrorMessageOnWrongWorkerAccess
	  {
		  get
		  {
			return "Bpmn error of External Task " + externalTaskId + " cannot be reported by worker '" + workerId;
		  }
	  }

	  public virtual void execute(ExternalTaskEntity externalTask)
	  {
		externalTask.bpmnError(errorCode, errorMessage, variables);
	  }
	}

}