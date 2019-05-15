using System;

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
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandContextListener = org.camunda.bpm.engine.impl.interceptor.CommandContextListener;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;

	public class JobFailureCollector : CommandContextListener
	{

	  protected internal Exception failure;
	  protected internal JobEntity job;
	  protected internal string jobId;

	  public JobFailureCollector(string jobId)
	  {
		this.jobId = jobId;
	  }

	  public virtual Exception Failure
	  {
		  set
		  {
			// log value if not already present
			if (this.failure == null)
			{
			  this.failure = value;
			}
		  }
		  get
		  {
			return failure;
		  }
	  }


	  public virtual void onCommandFailed(CommandContext commandContext, Exception t)
	  {
		Failure = t;
	  }

	  public virtual void onCommandContextClose(CommandContext commandContext)
	  {
		// ignore
	  }

	  public virtual JobEntity Job
	  {
		  set
		  {
			this.job = value;
		  }
		  get
		  {
			return job;
		  }
	  }


	  public virtual string JobId
	  {
		  get
		  {
			return jobId;
		  }
	  }

	}
}