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
namespace org.camunda.spin.plugin.impl
{

	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;

	/// <summary>
	/// A FunctionMapper which resolves the Spin functions for Expression Language.
	/// 
	/// <para>Lazy loading: This implementation supports lazy loading: the Java Methods
	/// are loaded upon the first request.</para>
	/// 
	/// <para>Caching: once the methods are loaded, they are cached in a Map for efficient
	/// retrieval.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpinFunctionMapper : FunctionMapper
	{

	  public static IDictionary<string, System.Reflection.MethodInfo> SPIN_FUNCTION_MAP = null;

	  public override System.Reflection.MethodInfo resolveFunction(string prefix, string localName)
	  {
		// Spin methods are used un-prefixed
		ensureSpinFunctionMapInitialized();
		return SPIN_FUNCTION_MAP[localName];
	  }

	  protected internal virtual void ensureSpinFunctionMapInitialized()
	  {
		if (SPIN_FUNCTION_MAP == null)
		{
		  lock (typeof(SpinFunctionMapper))
		  {
			if (SPIN_FUNCTION_MAP == null)
			{
			  SPIN_FUNCTION_MAP = new Dictionary<string, System.Reflection.MethodInfo>();
			  createMethodBindings();
			}
		  }
		}
	  }

	  protected internal virtual void createMethodBindings()
	  {
		Type spinClass = typeof(Spin);
		SPIN_FUNCTION_MAP["S"] = ReflectUtil.getMethod(spinClass, "S", typeof(object));
		SPIN_FUNCTION_MAP["XML"] = ReflectUtil.getMethod(spinClass, "XML", typeof(object));
		SPIN_FUNCTION_MAP["JSON"] = ReflectUtil.getMethod(spinClass, "JSON", typeof(object));
	  }

	}

}