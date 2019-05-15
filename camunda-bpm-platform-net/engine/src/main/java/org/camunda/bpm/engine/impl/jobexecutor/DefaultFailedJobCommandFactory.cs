﻿using System;

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
namespace org.camunda.bpm.engine.impl.jobexecutor
{
	using DefaultJobRetryCmd = org.camunda.bpm.engine.impl.cmd.DefaultJobRetryCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;


	public class DefaultFailedJobCommandFactory : FailedJobCommandFactory
	{

	  public virtual Command<object> getCommand(string jobId, Exception exception)
	  {
		return new DefaultJobRetryCmd(jobId, exception);
	  }

	}

}