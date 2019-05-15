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
namespace org.camunda.bpm.engine.impl.core.handler
{
	using CoreActivity = org.camunda.bpm.engine.impl.core.model.CoreActivity;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// <para>A <seealso cref="ModelElementHandler"/> handles an instance of a <seealso cref="ModelElementInstance modelElement"/>
	/// to create a new <seealso cref="CoreActivity activity."/></para>
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface ModelElementHandler<T, V, E> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance where V : HandlerContext
	{

	  /// <summary>
	  /// <para>This method handles a element to create a new element.</para>
	  /// </summary>
	  /// <param name="element"> the <seealso cref="ModelElementInstance"/> to be handled. </param>
	  /// <param name="context"> the <seealso cref="HandlerContext"/> which holds necessary information.
	  /// </param>
	  /// <returns> a new element. </returns>
	  E handleElement(T element, V context);

	}

}