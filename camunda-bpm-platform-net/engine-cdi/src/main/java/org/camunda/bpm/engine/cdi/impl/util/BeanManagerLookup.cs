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

	public class BeanManagerLookup
	{

	  /// <summary>
	  /// holds a local beanManager if no jndi is available </summary>
	  public static BeanManager localInstance;

	  /// <summary>
	  /// provide a custom jndi lookup name </summary>
	  public static string jndiName;

	  public static BeanManager BeanManager
	  {
		  get
		  {
    
			BeanManager beanManager = lookupBeanManagerInJndi();
    
			if (beanManager != null)
			{
			  return beanManager;
    
			}
			else
			{
			  if (localInstance != null)
			  {
				return localInstance;
			  }
			  else
			  {
				throw new ProcessEngineException("Could not lookup beanmanager in jndi. If no jndi is available, set the beanmanger to the 'localInstance' property of this class.");
			  }
			}
		  }
	  }

	  private static BeanManager lookupBeanManagerInJndi()
	  {

		if (!string.ReferenceEquals(jndiName, null))
		{
		  try
		  {
			return (BeanManager) InitialContext.doLookup(jndiName);
		  }
		  catch (NamingException e)
		  {
			throw new ProcessEngineException("Could not lookup beanmanager in jndi using name: '" + jndiName + "'.", e);
		  }
		}

		try
		{
		  // in an application server
		  return (BeanManager) InitialContext.doLookup("java:comp/BeanManager");
		}
		catch (NamingException)
		{
		  // silently ignore
		}

		try
		{
		  // in a servlet container
		  return (BeanManager) InitialContext.doLookup("java:comp/env/BeanManager");
		}
		catch (NamingException)
		{
		  // silently ignore
		}

		return null;

	  }
	}
}