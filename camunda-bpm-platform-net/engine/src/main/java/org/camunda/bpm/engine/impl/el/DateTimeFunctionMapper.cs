using System;
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
namespace org.camunda.bpm.engine.impl.el
{

	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using DateTime = org.joda.time.DateTime;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class DateTimeFunctionMapper : FunctionMapper
	{

	  public static IDictionary<string, System.Reflection.MethodInfo> DATE_TIME_FUNCTION_MAP = null;

	  public override System.Reflection.MethodInfo resolveFunction(string prefix, string localName)
	  {
		// Context functions are used un-prefixed
		ensureContextFunctionMapInitialized();
		return DATE_TIME_FUNCTION_MAP[localName];
	  }

	  protected internal virtual void ensureContextFunctionMapInitialized()
	  {
		if (DATE_TIME_FUNCTION_MAP == null)
		{
		  lock (typeof(CommandContextFunctionMapper))
		  {
			if (DATE_TIME_FUNCTION_MAP == null)
			{
			  DATE_TIME_FUNCTION_MAP = new Dictionary<string, System.Reflection.MethodInfo>();
			  createMethodBindings();
			}
		  }
		}
	  }

	  protected internal virtual void createMethodBindings()
	  {
		Type mapperClass = this.GetType();
		DATE_TIME_FUNCTION_MAP["now"] = ReflectUtil.getMethod(mapperClass, "now");
		DATE_TIME_FUNCTION_MAP["dateTime"] = ReflectUtil.getMethod(mapperClass, "dateTime");
	  }

	  public static DateTime now()
	  {
		return ClockUtil.CurrentTime;
	  }

	  public static DateTime dateTime()
	  {
		return new DateTime(now());
	  }

	}

}