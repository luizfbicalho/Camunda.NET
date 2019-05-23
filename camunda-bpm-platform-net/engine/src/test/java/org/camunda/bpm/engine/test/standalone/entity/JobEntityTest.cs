using System.Text;

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
namespace org.camunda.bpm.engine.test.standalone.entity
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// 
	/// <summary>
	/// @author Clint Manning
	/// </summary>
	public class JobEntityTest : PluggableProcessEngineTestCase
	{

	  /// <summary>
	  /// Note: This does not test a message with 4-byte Unicode supplementary
	  /// characters for two reasons:
	  /// - MySQL 5.1 does not support 4-byte supplementary characters (support from 5.5.3 onwards)
	  /// - <seealso cref="String.length()"/> counts these characters twice (since they are represented by two
	  /// chars), so essentially the cutoff would be half the actual cutoff for such a string
	  /// </summary>
	  public virtual void testInsertJobWithExceptionMessage()
	  {
		string fittingThreeByteMessage = repeatCharacter("\u9faf", StringUtil.DB_MAX_STRING_LENGTH);

		JobEntity threeByteJobEntity = new MessageEntity();
		threeByteJobEntity.ExceptionMessage = fittingThreeByteMessage;

		// should not fail
		insertJob(threeByteJobEntity);

		deleteJob(threeByteJobEntity);
	  }

	  public virtual void testJobExceptionMessageCutoff()
	  {
		JobEntity threeByteJobEntity = new MessageEntity();

		string message = repeatCharacter("a", StringUtil.DB_MAX_STRING_LENGTH * 2);
		threeByteJobEntity.ExceptionMessage = message;
		assertEquals(StringUtil.DB_MAX_STRING_LENGTH, threeByteJobEntity.ExceptionMessage.Length);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void insertJob(final org.camunda.bpm.engine.impl.persistence.entity.JobEntity jobEntity)
	  protected internal virtual void insertJob(JobEntity jobEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, jobEntity));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly JobEntityTest outerInstance;

		  private JobEntity jobEntity;

		  public CommandAnonymousInnerClass(JobEntityTest outerInstance, JobEntity jobEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.jobEntity = jobEntity;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			commandContext.JobManager.insert(jobEntity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void deleteJob(final org.camunda.bpm.engine.impl.persistence.entity.JobEntity jobEntity)
	  protected internal virtual void deleteJob(JobEntity jobEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, jobEntity));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly JobEntityTest outerInstance;

		  private JobEntity jobEntity;

		  public CommandAnonymousInnerClass2(JobEntityTest outerInstance, JobEntity jobEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.jobEntity = jobEntity;
		  }


		  public Void execute(CommandContext commandContext)
		  {
			commandContext.JobManager.delete(jobEntity);
			return null;
		  }
	  }

	  protected internal virtual string repeatCharacter(string encodedCharacter, int numCharacters)
	  {
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < numCharacters; i++)
		{
		  sb.Append(encodedCharacter);
		}

		return sb.ToString();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLongProcessDefinitionKey()
	   public virtual void testLongProcessDefinitionKey()
	   {
		string key = "myrealrealrealrealrealrealrealrealrealrealreallongprocessdefinitionkeyawesome";
		string processInstanceId = runtimeService.startProcessInstanceByKey(key).Id;

		Job job = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();

		assertEquals(key, job.ProcessDefinitionKey);
	   }

	}

}