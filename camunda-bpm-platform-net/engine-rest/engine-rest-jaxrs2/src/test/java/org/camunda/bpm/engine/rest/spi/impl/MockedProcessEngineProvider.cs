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
namespace org.camunda.bpm.engine.rest.spi.impl
{
	using ValueTypeResolverImpl = org.camunda.bpm.engine.impl.variable.ValueTypeResolverImpl;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using ValueTypeResolver = org.camunda.bpm.engine.variable.type.ValueTypeResolver;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	public class MockedProcessEngineProvider : ProcessEngineProvider
	{

	  private static ProcessEngine cachedDefaultProcessEngine;
	  private static IDictionary<string, ProcessEngine> cachedEngines = new Dictionary<string, ProcessEngine>();

	  public virtual void resetEngines()
	  {
		cachedDefaultProcessEngine = null;
		cachedEngines = new Dictionary<string, ProcessEngine>();
	  }

	  private ProcessEngine mockProcessEngine(string engineName)
	  {
		ProcessEngine engine = mock(typeof(ProcessEngine));
		when(engine.Name).thenReturn(engineName);
		mockServices(engine);
		mockProcessEngineConfiguration(engine);
		return engine;
	  }

	  private void mockServices(ProcessEngine engine)
	  {
		RepositoryService repoService = mock(typeof(RepositoryService));
		IdentityService identityService = mock(typeof(IdentityService));
		TaskService taskService = mock(typeof(TaskService));
		RuntimeService runtimeService = mock(typeof(RuntimeService));
		FormService formService = mock(typeof(FormService));
		HistoryService historyService = mock(typeof(HistoryService));
		ManagementService managementService = mock(typeof(ManagementService));
		CaseService caseService = mock(typeof(CaseService));
		FilterService filterService = mock(typeof(FilterService));
		ExternalTaskService externalTaskService = mock(typeof(ExternalTaskService));

		when(engine.RepositoryService).thenReturn(repoService);
		when(engine.IdentityService).thenReturn(identityService);
		when(engine.TaskService).thenReturn(taskService);
		when(engine.RuntimeService).thenReturn(runtimeService);
		when(engine.FormService).thenReturn(formService);
		when(engine.HistoryService).thenReturn(historyService);
		when(engine.ManagementService).thenReturn(managementService);
		when(engine.CaseService).thenReturn(caseService);
		when(engine.FilterService).thenReturn(filterService);
		when(engine.ExternalTaskService).thenReturn(externalTaskService);
	  }

	  protected internal virtual void mockProcessEngineConfiguration(ProcessEngine engine)
	  {
		ProcessEngineConfiguration configuration = mock(typeof(ProcessEngineConfiguration));
		when(configuration.ValueTypeResolver).thenReturn(mockValueTypeResolver());
		when(engine.ProcessEngineConfiguration).thenReturn(configuration);
	  }

	  protected internal virtual ValueTypeResolver mockValueTypeResolver()
	  {
		// no true mock here, but the impl class is a simple container and should be safe to use
		return new ValueTypeResolverImpl();
	  }

	  public virtual ProcessEngine DefaultProcessEngine
	  {
		  get
		  {
			if (cachedDefaultProcessEngine == null)
			{
			  cachedDefaultProcessEngine = mockProcessEngine("default");
			}
    
			return cachedDefaultProcessEngine;
		  }
	  }

	  public virtual ProcessEngine getProcessEngine(string name)
	  {
		if (name.Equals(MockProvider.NON_EXISTING_PROCESS_ENGINE_NAME))
		{
		  return null;
		}

		if (name.Equals("default"))
		{
		  return DefaultProcessEngine;
		}

		if (cachedEngines[name] == null)
		{
		  ProcessEngine mock = mockProcessEngine(name);
		  cachedEngines[name] = mock;
		}

		return cachedEngines[name];
	  }

	  public virtual ISet<string> ProcessEngineNames
	  {
		  get
		  {
			ISet<string> result = new HashSet<string>();
			result.Add(MockProvider.EXAMPLE_PROCESS_ENGINE_NAME);
			result.Add(MockProvider.ANOTHER_EXAMPLE_PROCESS_ENGINE_NAME);
			return result;
		  }
	  }


	}

}