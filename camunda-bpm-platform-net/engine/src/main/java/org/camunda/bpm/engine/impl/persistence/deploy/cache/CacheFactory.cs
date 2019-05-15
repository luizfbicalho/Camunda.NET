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
namespace org.camunda.bpm.engine.impl.persistence.deploy.cache
{
	using Cache = org.camunda.commons.utils.cache.Cache;

	/// <summary>
	/// <para>Builds the caches for the <seealso cref="DeploymentCache"/>.</para>
	/// </summary>
	public interface CacheFactory
	{

	  /// <summary>
	  /// Creates a cache that does not exceed a specified number of elements.
	  /// </summary>
	  /// <param name="maxNumberOfElementsInCache">
	  ///        The maximum number of elements that is allowed within the cache at the same time.
	  /// @return
	  ///        The cache to be created. </param>
	  Cache<string, T> createCache<T>(int maxNumberOfElementsInCache);
	}

}