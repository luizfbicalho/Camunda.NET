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

	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JobExecutorTestCase : PluggableProcessEngineTestCase
	{

	  protected internal TweetHandler tweetHandler = new TweetHandler();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public virtual void setUp()
	  {
		processEngineConfiguration.JobHandlers[tweetHandler.Type] = tweetHandler;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void tearDown() throws Exception
	  public virtual void tearDown()
	  {
		processEngineConfiguration.JobHandlers.Remove(tweetHandler.Type);
	  }

	  protected internal virtual MessageEntity createTweetMessage(string msg)
	  {
		MessageEntity message = new MessageEntity();
		message.JobHandlerType = "tweet";
		message.JobHandlerConfigurationRaw = msg;
		return message;
	  }

	  protected internal virtual TimerEntity createTweetTimer(string msg, DateTime duedate)
	  {
		TimerEntity timer = new TimerEntity();
		timer.JobHandlerType = "tweet";
		timer.JobHandlerConfigurationRaw = msg;
		timer.Duedate = duedate;
		return timer;
	  }

	}

}