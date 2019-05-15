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

	/// <summary>
	/// <para>An implementation of this context should contain necessary
	/// information to be accessed by a <seealso cref="ModelElementHandler"/>.</para>
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface HandlerContext
	{

	  /// <summary>
	  /// <para>This method returns an <seealso cref="CoreActivity activity"/>. The
	  /// returned activity represents a parent activity, which can
	  /// contain <seealso cref="CoreActivity activities"/>.</para>
	  /// 
	  /// <para>The returned activity should be used as a parent activity
	  /// for a new <seealso cref="CoreActivity activity"/>.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> a <seealso cref="CoreActivity"/> </returns>
	  CoreActivity Parent {get;}

	}

}