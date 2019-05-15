using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

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
namespace org.camunda.bpm.qa.upgrade
{

	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ScenarioRunner
	{

	  protected internal ProcessEngine engine;
	  protected internal string engineVersion;

	  public ScenarioRunner(ProcessEngine engine, string engineVersion)
	  {
		this.engine = engine;
		this.engineVersion = engineVersion;
	  }

	  public virtual void setupScenarios(Type clazz)
	  {
		performDeployments(clazz);
		executeScenarioSetups(clazz);
	  }

	  protected internal virtual void performDeployments(Type clazz)
	  {
		foreach (System.Reflection.MethodInfo method in clazz.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
		{
		  Deployment deploymentAnnotation = method.getAnnotation(typeof(Deployment));
		  if (deploymentAnnotation != null)
		  {
			object deploymentResource = null;
			try
			{
			  deploymentResource = method.invoke(null, new object[0]);
			}
			catch (Exception e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new Exception("Could not invoke method " + clazz.FullName + "#" + method.Name + " specifying a deployment", e);
			}

			if (method.ReturnType.IsAssignableFrom(typeof(string)))
			{
			  string deploymentResourcePath = (string) deploymentResource;
			  engine.RepositoryService.createDeployment().name(clazz.Name + "." + method.Name).addClasspathResource(deploymentResourcePath).deploy();
			}
			else if (method.ReturnType.IsAssignableFrom(typeof(BpmnModelInstance)))
			{
			  if (deploymentResource != null)
			  {
				BpmnModelInstance instance = (BpmnModelInstance) deploymentResource;
				engine.RepositoryService.createDeployment().addModelInstance(clazz.Name + "." + method.Name + ".bpmn20.xml", instance).deploy();
			  }
			}
		  }
		}
	  }

	  /// <summary>
	  /// Scans for all scenarios defined in the class and runs them
	  /// </summary>
	  protected internal virtual void executeScenarioSetups(Type clazz)
	  {
		IDictionary<string, Scenario> scenarios = new Dictionary<string, Scenario>();

		foreach (System.Reflection.MethodInfo method in clazz.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
		{
		  DescribesScenario scenarioAnnotation = method.getAnnotation(typeof(DescribesScenario));
		  if (scenarioAnnotation != null)
		  {
			Scenario scenario = new Scenario();

			scenario.Name = createScenarioName(clazz, scenarioAnnotation.value());
			ExtendsScenario extendedScenarioAnnotation = method.getAnnotation(typeof(ExtendsScenario));
			if (extendedScenarioAnnotation != null)
			{
			  scenario.ExtendedScenario = createScenarioName(clazz, extendedScenarioAnnotation.value());
			}

			try
			{
			  ScenarioSetup setup = (ScenarioSetup) method.invoke(null, new object[0]);
			  scenario.Setup = setup;
			}
			catch (Exception e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new Exception("Could not invoke method " + clazz.FullName + "#" + method.Name + " specifying scenarios " + scenario.Name, e);
			}

			Times timesAnnotation = method.getAnnotation(typeof(Times));
			if (timesAnnotation != null)
			{
			  scenario.Times = timesAnnotation.value();
			}

			scenarios[scenario.Name] = scenario;
		  }
		}

		foreach (Scenario scenario in scenarios.Values)
		{
		  setupScenario(scenarios, scenario);
		}
	  }

	  protected internal virtual string createScenarioName(Type declaringClass, string name)
	  {
		StringBuilder sb = new StringBuilder();

		if (!string.ReferenceEquals(engineVersion, null))
		{
		  sb.Append(engineVersion);
		  sb.Append(".");
		}

		sb.Append(declaringClass.Name);
		sb.Append(".");
		sb.Append(name);

		return sb.ToString();
	  }

	  protected internal virtual void setupScenario(IDictionary<string, Scenario> scenarios, Scenario scenario)
	  {
		scenario.createInstances(engine, scenarios);
	  }
	}

}