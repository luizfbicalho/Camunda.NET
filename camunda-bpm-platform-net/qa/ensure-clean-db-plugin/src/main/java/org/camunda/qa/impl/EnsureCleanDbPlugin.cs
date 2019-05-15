using System;
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
namespace org.camunda.qa.impl
{
	using BpmPlatform = org.camunda.bpm.BpmPlatform;
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using BpmPlatformPlugin = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugin;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using ManagementServiceImpl = org.camunda.bpm.engine.impl.ManagementServiceImpl;
	using DatabasePurgeReport = org.camunda.bpm.engine.impl.management.DatabasePurgeReport;
	using PurgeReport = org.camunda.bpm.engine.impl.management.PurgeReport;
	using CachePurgeReport = org.camunda.bpm.engine.impl.persistence.deploy.cache.CachePurgeReport;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Christopher Zell
	/// </summary>
	public class EnsureCleanDbPlugin : BpmPlatformPlugin
	{

	  protected internal const string DATABASE_NOT_CLEAN = "Database was not clean!\n";
	  protected internal const string CACHE_IS_NOT_CLEAN = "Cache was not clean!\n";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal Logger logger = Logger.getLogger(typeof(EnsureCleanDbPlugin).FullName);

	  private AtomicInteger counter = new AtomicInteger();

	  public virtual void postProcessApplicationDeploy(ProcessApplicationInterface processApplication)
	  {
		counter.incrementAndGet();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("resource") @Override public void postProcessApplicationUndeploy(org.camunda.bpm.application.ProcessApplicationInterface processApplication)
	  public virtual void postProcessApplicationUndeploy(ProcessApplicationInterface processApplication)
	  {
		// some tests deploy multiple PAs. => only clean DB after last PA is undeployed
		// if the deployment fails for example during parsing the deployment counter was not incremented
		// so we have to check if the counter is already zero otherwise we go into the negative values
		// best example is TestWarDeploymentWithBrokenBpmnXml in integration-test-engine test suite
		if (counter.get() == 0 || counter.decrementAndGet() == 0)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.ProcessEngine defaultProcessEngine = org.camunda.bpm.BpmPlatform.getDefaultProcessEngine();
		  ProcessEngine defaultProcessEngine = BpmPlatform.DefaultProcessEngine;
		  try
		  {
			logger.log(Level.INFO, "=== Ensure Clean Database ===");
			ManagementServiceImpl managementService = (ManagementServiceImpl) defaultProcessEngine.ManagementService;
			PurgeReport report = managementService.purge();

			if (report.Empty)
			{
			  logger.log(Level.INFO, "Clean DB and cache.");
			}
			else
			{
			  StringBuilder builder = new StringBuilder();

			  DatabasePurgeReport databasePurgeReport = report.DatabasePurgeReport;
			  if (!databasePurgeReport.Empty)
			  {
				builder.Append(DATABASE_NOT_CLEAN).Append(databasePurgeReport.PurgeReportAsString);
			  }

			  CachePurgeReport cachePurgeReport = report.CachePurgeReport;
			  if (!cachePurgeReport.Empty)
			  {
				builder.Append(CACHE_IS_NOT_CLEAN).Append(cachePurgeReport.PurgeReportAsString);
			  }
			  logger.log(Level.INFO, builder.ToString());
			}
		  }
		  catch (Exception e)
		  {
			logger.log(Level.SEVERE, "Could not clean DB:", e);
		  }
		}

	  }
	}

}