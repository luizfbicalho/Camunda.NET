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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using JobHandlerConfiguration = org.camunda.bpm.engine.impl.jobexecutor.JobHandlerConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using TweetJobConfiguration = org.camunda.bpm.engine.test.jobexecutor.TweetHandler.TweetJobConfiguration;
	using Assert = org.junit.Assert;

	public class TweetHandler : JobHandler<TweetJobConfiguration>
	{

	  internal IList<string> messages = new List<string>();

	  public virtual string Type
	  {
		  get
		  {
			return "tweet";
		  }
	  }

	  public virtual void execute(TweetJobConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {
		messages.Add(configuration.Message);
		Assert.assertNotNull(commandContext);
	  }

	  public virtual IList<string> Messages
	  {
		  get
		  {
			return messages;
		  }
	  }

	  public virtual TweetJobConfiguration newConfiguration(string canonicalString)
	  {
		TweetJobConfiguration config = new TweetJobConfiguration();
		config.message = canonicalString;

		return config;
	  }

	  public class TweetJobConfiguration : JobHandlerConfiguration
	  {
		protected internal string message;

		public virtual string Message
		{
			get
			{
			  return message;
			}
		}

		public virtual string toCanonicalString()
		{
		  return message;
		}
	  }

	  public virtual void onDelete(TweetJobConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }

	}

}