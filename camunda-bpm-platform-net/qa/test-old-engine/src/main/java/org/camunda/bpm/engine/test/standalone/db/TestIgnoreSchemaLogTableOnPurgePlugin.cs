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
namespace org.camunda.bpm.engine.test.standalone.db
{
	using ManagementServiceImpl = org.camunda.bpm.engine.impl.ManagementServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using PurgeReport = org.camunda.bpm.engine.impl.management.PurgeReport;

	/// <summary>
	/// Plugin that ensures that the new database table (ACT_GE_SCHEMA_LOG)
	/// introduced with 7.11 is ignored when running tests with the 7.10 engine and
	/// the 7.11 schema.
	/// TODO: Remove this after the 7.11 release.
	/// 
	/// @author Miklas Boskamp
	/// </summary>
	public class TestIgnoreSchemaLogTableOnPurgePlugin : ProcessEnginePlugin
	{

	  private class IgnoreSchemaLogTableOnPurgeManagementService : ManagementServiceImpl
	  {
		  private readonly TestIgnoreSchemaLogTableOnPurgePlugin outerInstance;

		  public IgnoreSchemaLogTableOnPurgeManagementService(TestIgnoreSchemaLogTableOnPurgePlugin outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		public override PurgeReport purge()
		{
		  return commandExecutor.execute(new TestIgnoreSchemaLogTablePurgeDatabaseAndCacheCmd());
		}
	  }

	  public virtual void preInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.ManagementService = new IgnoreSchemaLogTableOnPurgeManagementService(this);
	  }

	  public virtual void postInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
	  }

	  public virtual void postProcessEngineBuild(ProcessEngine processEngine)
	  {
	  }
	}
}