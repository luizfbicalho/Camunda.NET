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
namespace org.camunda.bpm.engine.test.concurrency.partitioning
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AttachmentEntity = org.camunda.bpm.engine.impl.persistence.entity.AttachmentEntity;
	using Attachment = org.camunda.bpm.engine.task.Attachment;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>

	public class CompetingHistoricAttachmentPartitioningTest : AbstractPartitioningTest
	{

	  public virtual void testConcurrentFetchAndDelete()
	  {
		// given
		string processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK).Id;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Attachment attachment = taskService.createAttachment("anAttachmentType", null, processInstanceId, "anAttachmentName", null, "http://camunda.com");
		Attachment attachment = taskService.createAttachment("anAttachmentType", null, processInstanceId, "anAttachmentName", null, "http://camunda.com");

		ThreadControl asyncThread = executeControllableCommand(new AsyncThread(this, attachment.Id));

		// assume
		assertThat(taskService.getAttachment(attachment.Id), notNullValue());

		asyncThread.waitForSync();

		commandExecutor.execute(new CommandAnonymousInnerClass(this, attachment));

		// when
		asyncThread.makeContinue();
		asyncThread.waitUntilDone();

		// then
		assertThat(taskService.getAttachment(attachment.Id), nullValue());
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly CompetingHistoricAttachmentPartitioningTest outerInstance;

		  private Attachment attachment;

		  public CommandAnonymousInnerClass(CompetingHistoricAttachmentPartitioningTest outerInstance, Attachment attachment)
		  {
			  this.outerInstance = outerInstance;
			  this.attachment = attachment;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			commandContext.AttachmentManager.delete((AttachmentEntity) attachment);

			return null;
		  }
	  }

	  public class AsyncThread : ControllableCommand<Void>
	  {
		  private readonly CompetingHistoricAttachmentPartitioningTest outerInstance;


		internal string attachmentId;

		internal AsyncThread(CompetingHistoricAttachmentPartitioningTest outerInstance, string attachmentId)
		{
			this.outerInstance = outerInstance;
		  this.attachmentId = attachmentId;
		}

		public override Void execute(CommandContext commandContext)
		{

		  commandContext.DbEntityManager.selectById(typeof(AttachmentEntity), attachmentId); // cache

		  monitor.sync();

		  AttachmentEntity changedAttachmentEntity = new AttachmentEntity();
		  changedAttachmentEntity.Id = attachmentId;

		  outerInstance.taskService.saveAttachment(changedAttachmentEntity);

		  return null;
		}

	  }

	}

}