﻿using System.Collections.Generic;

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

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public class SetExternalTaskRetriesCmd : ExternalTaskCmd
	{

	  protected internal int retries;
	  protected internal bool writeUserOperationLog;

	  public SetExternalTaskRetriesCmd(string externalTaskId, int retries, bool writeUserOperationLog) : base(externalTaskId)
	  {
		this.retries = retries;
		this.writeUserOperationLog = writeUserOperationLog;
	  }

	  protected internal override void validateInput()
	  {
		EnsureUtil.ensureGreaterThanOrEqual(typeof(BadUserRequestException), "The number of retries cannot be negative", "retries", retries, 0);
	  }

	  protected internal override void execute(ExternalTaskEntity externalTask)
	  {
		externalTask.RetriesAndManageIncidents = retries;
	  }

	  protected internal override string UserOperationLogOperationType
	  {
		  get
		  {
			if (writeUserOperationLog)
			{
			  return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_EXTERNAL_TASK_RETRIES;
			}
			return base.UserOperationLogOperationType;
		  }
	  }

	  protected internal override IList<PropertyChange> getUserOperationLogPropertyChanges(ExternalTaskEntity externalTask)
	  {
		if (writeUserOperationLog)
		{
		  return Collections.singletonList(new PropertyChange("retries", externalTask.Retries, retries));
		}
		return base.getUserOperationLogPropertyChanges(externalTask);
	  }
	}

}