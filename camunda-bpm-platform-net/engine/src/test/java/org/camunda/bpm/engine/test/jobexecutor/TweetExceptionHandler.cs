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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class TweetExceptionHandler : JobHandler<JobHandlerConfiguration>
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  public const string TYPE = "tweet-exception";

	  protected internal AtomicInteger exceptionsRemaining = new AtomicInteger(2);

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(JobHandlerConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		if (exceptionsRemaining.decrementAndGet() >= 0)
		{
		  throw new Exception("exception remaining: " + exceptionsRemaining);
		}
		LOG.info("no more exceptions to throw.");
	  }

	  public virtual JobHandlerConfiguration newConfiguration(string canonicalString)
	  {
		return new JobHandlerConfigurationAnonymousInnerClass(this);
	  }

	  private class JobHandlerConfigurationAnonymousInnerClass : JobHandlerConfiguration
	  {
		  private readonly TweetExceptionHandler outerInstance;

		  public JobHandlerConfigurationAnonymousInnerClass(TweetExceptionHandler outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public string toCanonicalString()
		  {
			return null;
		  }
	  }

	  public virtual void onDelete(JobHandlerConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	  public virtual int ExceptionsRemaining
	  {
		  get
		  {
			return exceptionsRemaining.get();
		  }
		  set
		  {
			this.exceptionsRemaining.set(value);
		  }
	  }


	}

}