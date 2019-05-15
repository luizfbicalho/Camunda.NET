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
namespace org.camunda.bpm.engine.test.cmmn.listener
{
	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SimpleBean
	{

	  protected internal static bool staticInvoked = false;

	  protected internal bool invoked = false;

	  public virtual void invoke()
	  {
		invoked = true;
	  }

	  public virtual bool wasInvoked()
	  {
		return invoked;
	  }

	  public static void invokeStatic()
	  {
		staticInvoked = true;
	  }

	  public static bool wasStaticallyInvoked()
	  {
		return staticInvoked;
	  }

	  public static void reset()
	  {
		staticInvoked = false;
	  }
	}

}