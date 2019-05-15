using System;

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
namespace org.camunda.bpm.engine.cdi.impl.util
{


	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandContextListener = org.camunda.bpm.engine.impl.interceptor.CommandContextListener;

	/// <summary>
	/// <seealso cref="CommandContextCloseListener"/> which releases a CDI Creational Context when the command context is closed.
	/// This is necessary to ensure that <seealso cref="Dependent"/> scoped beans are properly destroyed.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CreationalContextReleaseListener : CommandContextListener
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal static readonly Logger LOG = Logger.getLogger(typeof(CreationalContextReleaseListener).FullName);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected javax.enterprise.context.spi.CreationalContext<?> context;
	  protected internal CreationalContext<object> context;

	  public CreationalContextReleaseListener<T1>(CreationalContext<T1> ctx)
	  {
		context = ctx;
	  }

	  public virtual void onCommandContextClose(CommandContext commandContext)
	  {
		release(context);
	  }

	  public virtual void onCommandFailed(CommandContext commandContext, Exception t)
	  {
		// ignore
	  }

	  protected internal virtual void release<T1>(CreationalContext<T1> creationalContext)
	  {
		try
		{
		  creationalContext.release();
		}
		catch (Exception e)
		{
		  LOG.log(Level.WARNING, "Exception while releasing CDI creational context " + e.Message, e);
		}
	  }

	}

}