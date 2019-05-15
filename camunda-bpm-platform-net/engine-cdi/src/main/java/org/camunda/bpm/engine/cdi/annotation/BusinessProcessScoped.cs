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
namespace org.camunda.bpm.engine.cdi.annotation
{


	/// <summary>
	/// Declare a bean to be BusinessProcessScoped. Instances of
	/// BusinessProcessScoped beans are stored as process variables in a
	/// ProcessInstance.
	/// <p />
	/// Note: BusinessProcessScoped beans need to be <seealso cref="PassivationCapable"/>.
	/// <p />
	/// Note: BusinessProcessScoped is not capable of managing local process variables,
	/// and there is currently also no respective other implementation for that. Please use
	/// <seealso cref="org.camunda.bpm.engine.cdi.BusinessProcess#setVariableLocal(String, Object)"/>
	/// and <seealso cref="org.camunda.bpm.engine.cdi.BusinessProcess#getVariableLocal(String)"/>
	/// or an injected Map of local process variables instead.
	/// <p />
	/// If no ProcessInstance is currently managed, instances of
	/// <seealso cref="BusinessProcessScoped"/> beans are temporarily stored in a local scope
	/// (I.e. the Conversation or the Request, depending on the context, see javadoc
	/// on <seealso cref="ConversationScoped"/> and <seealso cref="RequestScoped"/> to find out when
	/// either context is active). If this scope is later associated with a business
	/// process instance, the bean instances are flushed to the ProcessInstance.
	/// <p />
	/// Example:
	/// <pre>
	/// {@code @BusinessProcessScoped}
	/// public class Authorization implements Serializable {
	///    ...
	/// }
	/// </pre>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class BusinessProcessScoped : System.Attribute
	{

	}

}