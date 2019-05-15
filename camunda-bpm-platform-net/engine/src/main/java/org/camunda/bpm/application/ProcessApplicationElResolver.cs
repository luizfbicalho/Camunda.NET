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
namespace org.camunda.bpm.application
{

	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;

	/// <summary>
	/// <para>SPI interface that allows providing a custom ElResolver implementation.</para>
	/// 
	/// <para>Implementations of this interface are looked up through the Java SE <seealso cref="ServiceLoader"/> facilities.
	/// If you want to provide a custom implementation in your application, place a file named
	/// <code>META-INF/org.camunda.bpm.application.ProcessApplicationElResolver</code> inside your application
	/// which contains the fully qualified classname of your implementation.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ProcessApplicationElResolver
	{

	  // precedences for known providers

	  /// <summary>
	  ///  Allows to set a precedence to the ElResolver. Resolver with a lower precedence will be invoked first.
	  /// </summary>
	  int? Precedence {get;}

	  /// <summary>
	  /// return the Resolver. May be null.
	  /// </summary>
	  ELResolver getElResolver(AbstractProcessApplication processApplication);

	  /// <summary>
	  /// Comparator used for sorting providers
	  /// </summary>
	  /// <seealso cref= ProcessApplicationElResolver#getPrecedence() </seealso>

	}

	public static class ProcessApplicationElResolver_Fields
	{
	  public const int SPRING_RESOLVER = 100;
	  public const int CDI_RESOLVER = 200;
	}

	  public class ProcessApplicationElResolver_ProcessApplicationElResolverSorter : IComparer<ProcessApplicationElResolver>
	  {

	public virtual int Compare(ProcessApplicationElResolver o1, ProcessApplicationElResolver o2)
	{
	  return (-1) * o1.Precedence.compareTo(o2.Precedence);
	}

	  }

}