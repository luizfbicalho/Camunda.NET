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
namespace org.camunda.bpm.engine.test.mock
{

	/// <summary>
	/// Registry for mock objects.
	/// 
	/// <para>Usage: <code>Mocks.register("myMock", myMock);</code></para>
	/// 
	/// <para>This class lets you register mock objects that will then be used by the
	/// <seealso cref="MockElResolver"/>. It binds a map of mock objects to ThreadLocal. This way, the 
	/// mocks can be set up independent of how the process engine configuration is built.</para>
	/// 
	/// @author Nils Preusker - n.preusker@gmail.com
	/// </summary>
	public class Mocks
	{

	  private static ThreadLocal<IDictionary<string, object>> mockContainer = new ThreadLocal<IDictionary<string, object>>();

	  public static IDictionary<string, object> getMocks()
	  {
		IDictionary<string, object> mocks = mockContainer.get();
		if (mocks == null)
		{
		  mocks = new Dictionary<string, object>();
		  Mocks.mockContainer.set(mocks);
		}
		return mocks;
	  }

	  /// <summary>
	  /// This method lets you register a mock object. Make sure to register the
	  /// <seealso cref="MockExpressionManager"/> with your process engine configuration.
	  /// </summary>
	  /// <param name="key">
	  ///          the key under which the mock object will be registered </param>
	  /// <param name="value">
	  ///          the mock object </param>
	  public static void register(string key, object value)
	  {
		getMocks()[key] = value;
	  }

	  /// <summary>
	  /// This method returns the mock object registered under the provided key or
	  /// null if there is no object for the provided key.
	  /// </summary>
	  /// <param name="key">
	  ///          the key of the requested object </param>
	  /// <returns> the mock object registered under the provided key or null if there
	  ///         is no object for the provided key </returns>
	  public static object get(object key)
	  {
		return getMocks()[key];
	  }

	  /// <summary>
	  /// This method resets the internal map of mock objects.
	  /// </summary>
	  public static void reset()
	  {
		if (getMocks() != null)
		{
		  getMocks().Clear();
		}
	  }

	}

}