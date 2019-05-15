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
namespace org.camunda.bpm.qa.performance.engine.sqlstatementlog
{

	using PerfTest = org.camunda.bpm.qa.performance.engine.framework.PerfTest;
	using PerfTestPass = org.camunda.bpm.qa.performance.engine.framework.PerfTestPass;
	using PerfTestRun = org.camunda.bpm.qa.performance.engine.framework.PerfTestRun;
	using PerfTestStep = org.camunda.bpm.qa.performance.engine.framework.PerfTestStep;
	using PerfTestWatcher = org.camunda.bpm.qa.performance.engine.framework.PerfTestWatcher;
	using SqlStatementLog = org.camunda.bpm.qa.performance.engine.sqlstatementlog.StatementLogSqlSession.SqlStatementLog;

	/// <summary>
	/// <seealso cref="PerfTestWatcher"/> performing statement logging.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StatementLogPerfTestWatcher : PerfTestWatcher
	{

	  public virtual void beforePass(PerfTestPass pass)
	  {
		// nothing to do
	  }

	  public virtual void beforeRun(PerfTest test, PerfTestRun run)
	  {
		// nothing to do
	  }

	  public virtual void beforeStep(PerfTestStep step, PerfTestRun run)
	  {
		StatementLogSqlSession.startLogging();
	  }

	  public virtual void afterStep(PerfTestStep step, PerfTestRun run)
	  {
		IList<SqlStatementLog> loggedStatements = StatementLogSqlSession.stopLogging();
		run.logStepResult(loggedStatements);
	  }

	  public virtual void afterRun(PerfTest test, PerfTestRun run)
	  {
		// nothing to do
	  }

	  public virtual void afterPass(PerfTestPass pass)
	  {
		// nothing to do
	  }

	}

}