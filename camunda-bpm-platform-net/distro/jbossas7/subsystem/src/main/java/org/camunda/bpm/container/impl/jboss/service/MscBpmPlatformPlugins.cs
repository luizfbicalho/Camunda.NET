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
namespace org.camunda.bpm.container.impl.jboss.service
{
	using BpmPlatformPlugins = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins;
	using Service = org.jboss.msc.service.Service;
	using StartContext = org.jboss.msc.service.StartContext;
	using StartException = org.jboss.msc.service.StartException;
	using StopContext = org.jboss.msc.service.StopContext;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MscBpmPlatformPlugins : Service<BpmPlatformPlugins>
	{

	  protected internal BpmPlatformPlugins plugins;

	  public MscBpmPlatformPlugins(BpmPlatformPlugins plugins)
	  {
		this.plugins = plugins;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins getValue() throws IllegalStateException, IllegalArgumentException
	  public override BpmPlatformPlugins Value
	  {
		  get
		  {
			return plugins;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void start(org.jboss.msc.service.StartContext context) throws org.jboss.msc.service.StartException
	  public override void start(StartContext context)
	  {
		// nothing to do
	  }

	  public override void stop(StopContext context)
	  {
		// nothing to do
	  }

	}

}