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
namespace org.camunda.bpm.engine.test.api.authorization.util
{
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandInterceptor;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AuthorizationExceptionInterceptor : CommandInterceptor
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  protected internal bool isActive;
	  protected internal AuthorizationException lastException;

	  protected internal int count = 0;

	  public override T execute<T>(Command<T> command)
	  {
		try
		{
		  count++; // only catch exception if we are at the top of the command stack
				   // (there may be multiple nested command invocations and we need
				   // to prevent that this intercepter swallows an exception)
		  T result = next.execute(command);
		  count--;
		  return result;
		}
		catch (AuthorizationException e)
		{
		  count--;
		  if (count == 0 && isActive)
		  {
			lastException = e;
			LOG.info("Caught authorization exception; storing for assertion in test", e);
		  }
		  else
		  {
			throw e;
		  }
		}
		return default(T);
	  }

	  public virtual void reset()
	  {
		lastException = null;
		count = 0;
	  }

	  public virtual AuthorizationException LastException
	  {
		  get
		  {
			return lastException;
		  }
	  }

	  public virtual void activate()
	  {
		isActive = true;
	  }

	  public virtual void deactivate()
	  {
		isActive = false;
	  }
	}

}