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
namespace org.camunda.bpm.engine.test.concurrency
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class ThrowOleWhenDeletingExceptionStacktraceTest : ConcurrencyTestCase
	{

	  protected internal AtomicReference<JobEntity> job = new AtomicReference<JobEntity>();

	  protected internal virtual void runTest()
	  {
		// ignored
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		if (job.get() != null)
		{
		  processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		}

		base.tearDown();
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly ThrowOleWhenDeletingExceptionStacktraceTest outerInstance;

		  public CommandAnonymousInnerClass(ThrowOleWhenDeletingExceptionStacktraceTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			JobEntity jobEntity = outerInstance.job.get();

			jobEntity.Revision = 2;

			commandContext.JobManager.deleteJob(jobEntity);
			commandContext.ByteArrayManager.deleteByteArrayById(jobEntity.ExceptionByteArrayId);
			commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobEntity.Id);

			return null;
		  }
	  }

	  public virtual void testThrowOleWhenDeletingExceptionStacktraceTest()
	  {
		// given
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));

		ThreadControl threadOne = executeControllableCommand(new ThreadOne(this));

		ThreadControl threadTwo = executeControllableCommand(new ThreadTwo(this));
		threadTwo.reportInterrupts();

		threadOne.waitForSync();
		threadTwo.waitForSync();

		threadTwo.makeContinueAndWaitForSync(); // store job entity in cache

		threadOne.makeContinue(); // flush job update statement (REV_ + 1)
		threadOne.join();

		// when
		threadTwo.makeContinue(); // flush delete statements (REV_ - 1)
		threadTwo.join();

		// then
		assertThat(threadTwo.Exception.Message).contains("Entity was updated by another transaction concurrently.");
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly ThrowOleWhenDeletingExceptionStacktraceTest outerInstance;

		  public CommandAnonymousInnerClass2(ThrowOleWhenDeletingExceptionStacktraceTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			JobEntity jobEntity = new MessageEntity();

			jobEntity.ExceptionStacktrace = "foo";

			commandContext.JobManager.insert(jobEntity);

			outerInstance.job.set(jobEntity);

			return null;
		  }
	  }

	  public class ThreadOne : ControllableCommand<Void>
	  {
		  private readonly ThrowOleWhenDeletingExceptionStacktraceTest outerInstance;

		  public ThreadOne(ThrowOleWhenDeletingExceptionStacktraceTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public override Void execute(CommandContext commandContext)
		{
		  monitor.sync();

		  JobEntity jobEntity = commandContext.JobManager.findJobById(outerInstance.job.get().Id);
		  jobEntity.LockOwner = "foo";

		  return null;
		}

	  }

	  public class ThreadTwo : ControllableCommand<Void>
	  {
		  private readonly ThrowOleWhenDeletingExceptionStacktraceTest outerInstance;

		  public ThreadTwo(ThrowOleWhenDeletingExceptionStacktraceTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public override Void execute(CommandContext commandContext)
		{
		  monitor.sync();

		  JobEntity jobEntity = commandContext.JobManager.findJobById(outerInstance.job.get().Id);

		  monitor.sync();

		  commandContext.JobManager.deleteJob(jobEntity);

		  string byteArrayId = jobEntity.ExceptionByteArrayId;
		  commandContext.ByteArrayManager.deleteByteArrayById(byteArrayId);

		  return null;
		}

	  }

	}

}