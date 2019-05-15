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
namespace org.camunda.bpm.engine.impl.cfg
{

	using CommandContextInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandContextInterceptor;
	using CommandInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandInterceptor;
	using LogInterceptor = org.camunda.bpm.engine.impl.interceptor.LogInterceptor;
	using ProcessApplicationContextInterceptor = org.camunda.bpm.engine.impl.interceptor.ProcessApplicationContextInterceptor;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class StandaloneProcessEngineConfiguration : ProcessEngineConfigurationImpl
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Collection< ? extends org.camunda.bpm.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptorsTxRequired()
	  protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequired
	  {
		  get
		  {
			IList<CommandInterceptor> defaultCommandInterceptorsTxRequired = new List<CommandInterceptor>();
			defaultCommandInterceptorsTxRequired.Add(new LogInterceptor());
			defaultCommandInterceptorsTxRequired.Add(new ProcessApplicationContextInterceptor(this));
			defaultCommandInterceptorsTxRequired.Add(new CommandContextInterceptor(commandContextFactory, this));
			return defaultCommandInterceptorsTxRequired;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Collection< ? extends org.camunda.bpm.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptorsTxRequiresNew()
	  protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequiresNew
	  {
		  get
		  {
			IList<CommandInterceptor> defaultCommandInterceptorsTxRequired = new List<CommandInterceptor>();
			defaultCommandInterceptorsTxRequired.Add(new LogInterceptor());
			defaultCommandInterceptorsTxRequired.Add(new ProcessApplicationContextInterceptor(this));
			defaultCommandInterceptorsTxRequired.Add(new CommandContextInterceptor(commandContextFactory, this, true));
			return defaultCommandInterceptorsTxRequired;
		  }
	  }

	}

}